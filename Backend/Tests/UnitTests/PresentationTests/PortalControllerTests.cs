using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces.Portal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Presentation.Controllers;

namespace PresentationTests
{
    [TestFixture]
    public class PortalControllerTests
    {
        private readonly Fixture _fix = new();
        private static readonly ControllerSettings _settings = new()
        {
            DefaultPageSize = 16,
            SmallPageSize = 8,
            BigPageSize = 32
        };
        private static PortalController CreateController(IPortalService service)
            => new(service, Options.Create(_settings));

        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(null, 1)]
        [TestCase(1, 1)]
        public async Task GetSubjectsPageForCurrentUserAsync_Success(
                int? lastId,
                int? pageSize)
        {
            var subjects = _fix.CreateMany<SubjectDto>()
                .ToList();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.GetSubjectsPageForCurrentUserAsync(
                        lastId, It.IsAny<int>()))
                .ReturnsAsync(subjects);
            var sut = CreateController(mock.Object);

            var res = await sut.GetSubjectsPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as List<SubjectDto>;
            Assert.That(actual, Is.EquivalentTo(subjects));
        }


        [Test]
        public async Task GetSubjectByIdAsync_Success()
        {
            var subject = _fix.Create<SubjectFullDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.GetFullSubjectByIdAsync(subject.Id))
                .ReturnsAsync(subject);
            var sut = CreateController(mock.Object);

            var res = await sut.GetSubjectByIdAsync(subject.Id);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as SubjectFullDto;
            Assert.That(actual, Is.EqualTo(subject));
        }

        [Test]
        public async Task AddSubjectAsync_Success()
        {
            var request = _fix.Create<SubjectRequestDto>();
            var subjectDto = _fix.Create<SubjectFullDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.AddSubjectForCurrentUserAsync(request))
                .ReturnsAsync(subjectDto);
            var sut = CreateController(mock.Object);

            var res = await sut.AddSubjectAsync(request);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as SubjectFullDto;
            Assert.That(actual, Is.EqualTo(subjectDto));
        }


        [Test]
        public async Task DeleteSubjectByIdAsync_Success()
        {
            var subjectDto = _fix.Create<SubjectDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.RemoveSubjectByIdAsync(subjectDto.Id))
                .ReturnsAsync(subjectDto);
            var sut = CreateController(mock.Object);

            var res = await sut.RemoveSubjectByIdAsync(subjectDto.Id);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as SubjectDto;
            Assert.That(actual, Is.EqualTo(subjectDto));
        }


        [Test]
        public async Task AddScheduleAsync_Success()
        {
            var request = _fix.Create<ScheduleRequestDto>();
            var scheduleDto = _fix.Create<ScheduleDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.AddScheduleForSubjectAsync(request))
                .ReturnsAsync(scheduleDto);
            var sut = CreateController(mock.Object);

            var res = await sut.AddScheduleAsync(request);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as ScheduleDto;
            Assert.That(actual, Is.EqualTo(scheduleDto));
        }


        [Test]
        public async Task RemoveScheduleAsync_Success()
        {
            var scheduleDto = _fix.Create<ScheduleDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.RemoveScheduleByIdAsync(scheduleDto.Id))
                .ReturnsAsync(scheduleDto);
            var sut = CreateController(mock.Object);

            var res = await sut.RemoveScheduleAsync(scheduleDto.Id);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as ScheduleDto;
            Assert.That(actual, Is.EqualTo(scheduleDto));
        }


        [Test]
        public async Task AddLecturerAsync_Success()
        {
            var request = _fix.Create<LecturerRequestDto>();
            var lecturerDto = _fix.Create<LecturerDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.AddLecturerToSubjectAsync(request))
                .ReturnsAsync(lecturerDto);
            var sut = CreateController(mock.Object);

            var res = await sut.AddLecturerAsync(request);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as LecturerDto;
            Assert.That(actual, Is.EqualTo(lecturerDto));
        }


        [Test]
        public async Task RemoveLecturerAsync_Success()
        {
            var lecturerDto = _fix.Create<LecturerDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.RemoveLecturerByIdAsync(lecturerDto.Id))
                .ReturnsAsync(lecturerDto);
            var sut = CreateController(mock.Object);

            var res = await sut.RemoveLecturerAsync(lecturerDto.Id);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as LecturerDto;
            Assert.That(actual, Is.EqualTo(lecturerDto));
        }


        [Test]
        public async Task AddSyllabusAsync_Success()
        {
            var request = _fix.Create<SyllabusRequestDto>();
            var testDto = _fix.Create<SyllabusDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.AddSylabusForSubjectAsync(request))
                .ReturnsAsync(testDto);
            var sut = CreateController(mock.Object);

            var res = await sut.AddSylabusAsync(request);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as SyllabusDto;
            Assert.That(actual, Is.EqualTo(testDto));
        }


        [Test]
        public async Task GetSyllabusByIdAsync_Success()
        {
            var testDto = _fix.Create<SyllabusFullDto>();
            var mock = new Mock<IPortalService>();
            mock.Setup(s => s.GetFullSyllabusByIdAsync(testDto.Id))
                .ReturnsAsync(testDto);
            var sut = CreateController(mock.Object);

            var res = await sut.GetSylabusByIdAsync(testDto.Id);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as SyllabusFullDto;
            Assert.That(actual, Is.EqualTo(testDto));
        }

    }
}

