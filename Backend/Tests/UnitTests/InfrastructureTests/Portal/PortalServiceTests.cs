using AutoFixture;
using Core.Dtos;
using Core.Entities.Portal;
using Core.Exceptions;
using Core.Interfaces.Auth;
using Core.Interfaces.Portal;
using Infrastructure.Services.Portal;
using Moq;

namespace InfrastructureTests.Portal
{
    [TestFixture]
    public class PortalServiceTests
    {
        private const int DEFAULT_CONTEXT_ID = 9845;

        private static readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        private readonly IPortalMapper _mapper = new PortalMapper();

        private IPortalService CreateService(
                IPortalRepo? repo = null,
                ICurrentContext? currentContext = null
                )
        {
            repo ??= new Mock<IPortalRepo>().Object;
            if (currentContext is null)
            {
                var mock = new Mock<ICurrentContext>();
                mock.Setup(c => c.GetCurrentUserId())
                    .Returns(DEFAULT_CONTEXT_ID);
                currentContext = mock.Object;
            }
            return new PortalService(repo, _mapper, currentContext);
        }


        [Test]
        public async Task AddSubjectForCurrentUser_Success()
        {
            var dto = _fix.Create<SubjectRequestDto>();
            var subject = _mapper.ToSubject(dto, DEFAULT_CONTEXT_ID);
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.AddSubjectForUserAsync(It.IsAny<Subject>()))
                .ReturnsAsync((Subject s) => s);
            var sut = CreateService(repoMock.Object);

            var res = await sut.AddSubjectForCurrentUser(dto);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(dto.Name));
        }



        [Test]
        public async Task RemoveSybjectById_Success()
        {
            var dto = _fix.Create<SubjectRequestDto>();
            var subject = _mapper.ToSubject(dto, DEFAULT_CONTEXT_ID);
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.RemoveSubjectById(subject.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(subject);
            var sut = CreateService(repoMock.Object);

            var res = await sut.RemoveSubjectById(subject.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(dto.Name));
        }



        [Test]
        public async Task GetSubjectsPageForCurrentUserAsync_Success()
        {
            var subjects = _fix.CreateMany<Subject>().ToList();
            var subjectDtos = subjects.ConvertAll(_mapper.ToSubjectDto);
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.GetSubjectsPageForUserAsync(DEFAULT_CONTEXT_ID,
                        null, It.IsAny<int>()))
                .ReturnsAsync(subjects);
            var sut = CreateService(repoMock.Object);

            var res = await sut.GetSubjectsPageForCurrentUserAsync(null, 16);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ConvertAll(s => s.Name).Order(),
                    Is.EqualTo(subjects.ConvertAll(s => s.Name).Order()));
        }



        [Test]
        public async Task GetFullSubjectById_Success()
        {
            var subject = _fix.Create<Subject>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.GetFullSubjectByIdAsync(subject.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(subject);
            var sut = CreateService(repoMock.Object);

            var res = await sut.GetFullSubjectById(subject.Id);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, subject);
        }


        [Test]
        public async Task GetFullSubjectById_NotFound_Throws()
        {
            var subject = _fix.Create<Subject>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.GetFullSubjectByIdAsync(subject.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(() => null);
            var sut = CreateService(repoMock.Object);

            Assert.ThrowsAsync<SubjectNotFoundException>(async () =>
                    await sut.GetFullSubjectById(subject.Id));
        }

    }
}
