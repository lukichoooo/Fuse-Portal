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
                ICurrentContext? currentContext = null,
                IPortalParser? portalParser = null,
                IMockExamService? examService = null
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
                var portalDto = _fix.Create<PortalParserResponseDto>();
                var mock = new Mock<IPortalParser>();
                mock.Setup(c => c.ParsePortalHtml(It.IsAny<string>()))
                    .ReturnsAsync(portalDto);
                portalParser = mock.Object;
            }
            if (examService is null)
            {
                var mock = new Mock<IMockExamService>();
                examService = mock.Object;
            }

            return new PortalService(
                    repo,
                    _mapper,
                    currentContext,
                    portalParser,
                    examService);
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
            var request = _fix.Create<SyllabusRequestDto>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.AddSyllabusForSubjectAsync(
                        It.IsAny<Syllabus>(), DEFAULT_CONTEXT_ID))
                .ReturnsAsync((Syllabus t, int _) => t);
            var sut = CreateService(repoMock.Object);

            var res = await sut.AddSylabusForSubjectAsync(request);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(request.Name));
        }


        [Test]
        public async Task RemoveTestByIdAsync_Success()
        {
            var test = _fix.Create<Syllabus>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.RemoveSyllabusByIdAsync(
                        test.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(test);
            var sut = CreateService(repoMock.Object);

            var res = await sut.RemoveSyllabusByIdAsync(test.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(test.Name));
        }



        [Test]
        public async Task GetFullTestByIdAsync_Success()
        {
            var test = _fix.Create<Syllabus>();
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.GetFullSyllabusByIdAsync(
                        test.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(test);
            var sut = CreateService(repoMock.Object);

            var res = await sut.GetFullSyllabusByIdAsync(test.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(test.Name));
            Assert.That(res.Content, Is.EqualTo(test.Content));
        }

        [Test]
        public async Task ParseAndSavePortalAsync_Success()
        {
            var fixture = new Fixture() { RepeatCount = 100 };
            var htmlPage = fixture.Create<string>();
            var portalDto = _fix.Create<PortalParserResponseDto>();
            var parserMock = new Mock<IPortalParser>();
            parserMock.Setup(p => p.ParsePortalHtml(htmlPage))
                .ReturnsAsync(portalDto);
            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.AddSubjectForUserAsync(It.IsAny<Subject>()))
                .ReturnsAsync((Subject s) => s);
            var sut = CreateService(
                    repo: repoMock.Object,
                    portalParser: parserMock.Object);

            var res = await sut.ParseAndSavePortalAsync(htmlPage);

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

            int testCount = portalDto.Subjects.Sum(s => s.Syllabuses.Count);
            repoMock.Verify(r => r.AddSyllabusForSubjectAsync(
                        It.IsAny<Syllabus>(),
                        DEFAULT_CONTEXT_ID),
                    Times.Exactly(testCount));
        }

        [Test]
        public async Task GenerateMockExamForSyllabusAsync_Success()
        {
            var syllabus = _fix.Create<Syllabus>();
            var questions = _fix.Create<string>();

            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.GetFullSyllabusByIdAsync(
                        syllabus.Id, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(syllabus);
            repoMock.Setup(r => r.AddExamAsync(
                        It.IsAny<Exam>(), DEFAULT_CONTEXT_ID))
                .ReturnsAsync((Exam e, int _) => e);

            var examServiceMock = new Mock<IMockExamService>();
            examServiceMock.Setup(c => c.GenerateExamQuestionsAsync(syllabus.Content))
                .ReturnsAsync(questions);

            var sut = CreateService(
                    repo: repoMock.Object,
                    examService: examServiceMock.Object
                    );

            var res = await sut.GenerateMockExamForSyllabusAsync(syllabus.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Questions, Is.EqualTo(questions));
        }


        [Test]
        public async Task CheckExamAnswersAsync_Success()
        {
            var examDto = _fix.Create<ExamDto>();

            var examServiceMock = new Mock<IMockExamService>();
            examServiceMock.Setup(c => c.GetExamResultsAsync(It.IsAny<ExamDto>()))
                .ReturnsAsync((ExamDto e) => e);

            var repoMock = new Mock<IPortalRepo>();
            repoMock.Setup(r => r.UpdateExamResultsAsync(
                        It.IsAny<Exam>(), DEFAULT_CONTEXT_ID))
                .ReturnsAsync((Exam e, int _) => e);

            var sut = CreateService(
                    repo: repoMock.Object,
                    examService: examServiceMock.Object);

            var res = await sut.CheckExamAnswersAsync(examDto);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Questions, Is.EqualTo(examDto.Questions));
        }


    }
}
