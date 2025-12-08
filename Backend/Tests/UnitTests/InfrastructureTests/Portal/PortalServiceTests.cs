using AutoFixture;
using Core.Dtos;
using Core.Entities.Portal;
using Core.Exceptions;
using Core.Interfaces.Auth;
using Core.Interfaces.Portal;
using Infrastructure.Services.Portal;
using Moq;
using NPOI.SS.Formula.Functions;

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
                ICurrentContext? currentContext = null,
                IPortalParser? portalParser = null
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
            if (portalParser is null)
            {
                var portalDto = _fix.Create<PortalParserDto>();
                var mock = new Mock<IPortalParser>();
                mock.Setup(c => c.ParsePortalHtml(It.IsAny<ParsePortalRequest>()))
                    .ReturnsAsync(portalDto);
                portalParser = mock.Object;
            }
            return new PortalService(
                    repo,
                    _mapper,
                    currentContext,
                    portalParser);
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

            var res = await sut.AddSubjectForCurrentUserAsync(dto);

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

            var res = await sut.RemoveSubjectByIdAsync(subject.Id);

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

            var res = await sut.GetFullSubjectByIdAsync(subject.Id);

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
                    await sut.GetFullSubjectByIdAsync(subject.Id));
        }

        [Test]
        public async Task AddScheduleForSubjectAsync_Success()
        {
            var request = _fix.Create<ScheduleRequestDto>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.AddScheduleForSubjectAsync(
                        It.IsAny<Schedule>(), DEFAULT_CONTEXT_ID))
                .ReturnsAsync((Schedule s, int _) => s);
            var sut = CreateService(repoMock.Object);

            var res = await sut.AddScheduleForSubjectAsync(request);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Metadata, Is.EqualTo(request.Metadata));
        }


        [Test]
        public async Task RemoveScheduleById_Success()
        {
            var schedule = _fix.Create<Schedule>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.RemoveScheduleByIdAsync(
                        schedule.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(schedule);
            var sut = CreateService(repoMock.Object);

            var res = await sut.RemoveScheduleByIdAsync(schedule.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(schedule.Id));
        }


        [Test]
        public async Task AddLecturerToSubject_Success()
        {
            var request = _fix.Create<LecturerRequestDto>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.AddLecturerToSubjectAsync(
                        It.IsAny<Lecturer>(), DEFAULT_CONTEXT_ID))
                .ReturnsAsync((Lecturer l, int _) => l);
            var sut = CreateService(repoMock.Object);

            var res = await sut.AddLecturerToSubjectAsync(request);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(request.Name));
        }

        [Test]
        public async Task RemoveLecturerByIdAsync_Success()
        {
            var lecturerId = _fix.Create<int>();
            var lecturer = _fix.Create<Lecturer>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.RemoveLecturerByIdAsync(
                        lecturerId, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(lecturer);
            var sut = CreateService(repoMock.Object);

            var res = await sut.RemoveLecturerByIdAsync(lecturerId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(lecturer.Name));
        }


        [Test]
        public async Task AddTestForSubjectAsync_Success()
        {
            var request = _fix.Create<TestRequestDto>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.AddTestForSubjectAsync(
                        It.IsAny<Test>(), DEFAULT_CONTEXT_ID))
                .ReturnsAsync((Test t, int _) => t);
            var sut = CreateService(repoMock.Object);

            var res = await sut.AddTestForSubjectAsync(request);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(request.Name));
        }


        [Test]
        public async Task RemoveTestByIdAsync_Success()
        {
            var test = _fix.Create<Test>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.RemoveTestByIdAsync(
                        test.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(test);
            var sut = CreateService(repoMock.Object);

            var res = await sut.RemoveTestByIdAsync(test.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(test.Name));
        }



        [Test]
        public async Task GetFullTestByIdAsync_Success()
        {
            var test = _fix.Create<Test>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.GetFullTestByIdAsync(
                        test.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(test);
            var sut = CreateService(repoMock.Object);

            var res = await sut.GetFullTestByIdAsync(test.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(test.Name));
            Assert.That(res.Content, Is.EqualTo(test.Content));
        }

        [Test]
        public async Task ParseAndSavePortalAsync_Success()
        {
            var fixture = new Fixture() { RepeatCount = 100 };
            var parsePortalRequest = fixture.Create<ParsePortalRequest>();
            var portalDto = _fix.Create<PortalParserDto>();
            var parserMock = new Mock<IPortalParser>();
            parserMock.Setup(p => p.ParsePortalHtml(parsePortalRequest))
                .ReturnsAsync(portalDto);
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.AddSubjectForUserAsync(It.IsAny<Subject>()))
                .ReturnsAsync((Subject s) => s);
            var sut = CreateService(
                    repo: repoMock.Object,
                    portalParser: parserMock.Object);

            var res = await sut.ParseAndSavePortalAsync(parsePortalRequest);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(portalDto));

            repoMock.Verify(r => r.AddSubjectForUserAsync(It.IsAny<Subject>()),
                    Times.Exactly(portalDto.Subjects.Count));

            int schedulesCount = portalDto.Subjects.Sum(s => s.Schedules.Count);
            repoMock.Verify(r => r.AddScheduleForSubjectAsync(
                        It.IsAny<Schedule>(),
                        DEFAULT_CONTEXT_ID),
                    Times.Exactly(schedulesCount));

            int lecturerCount = portalDto.Subjects.Sum(s => s.Lecturers.Count);
            repoMock.Verify(r => r.AddLecturerToSubjectAsync(
                        It.IsAny<Lecturer>(),
                        DEFAULT_CONTEXT_ID),
                    Times.Exactly(lecturerCount));

            int testCount = portalDto.Subjects.Sum(s => s.Tests.Count);
            repoMock.Verify(r => r.AddTestForSubjectAsync(
                        It.IsAny<Test>(),
                        DEFAULT_CONTEXT_ID),
                    Times.Exactly(testCount));
        }

    }
}
