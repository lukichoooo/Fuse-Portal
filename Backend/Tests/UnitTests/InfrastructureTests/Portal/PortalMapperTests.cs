using AutoFixture;
using Core.Dtos;
using Core.Entities.Portal;
using Core.Interfaces.Portal;
using Infrastructure.Services.Portal;

namespace InfrastructureTests.Portal
{
    [TestFixture]
    public class PortalMapperTests
    {
        private readonly IPortalMapper _sut = new PortalMapper();
        private readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }



        [Test]
        public void ToScheduleDto_From_Schedule()
        {
            var schedule = _fix.Create<Schedule>();
            var res = _sut.ToScheduleDto(schedule);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, schedule);
        }


        [Test]
        public void ToSubject_From_SubjectDto()
        {
            var dto = _fix.Create<SubjectRequestDto>();
            var userId = _fix.Create<int>();
            var res = _sut.ToSubject(dto, userId);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, dto);
        }


        [Test]
        public void ToSubjectDto_From_Subject()
        {
            var subject = _fix.Create<Subject>();
            var res = _sut.ToSubjectDto(subject);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, subject);
        }


        [Test]
        public void ToTestDto_From_Test()
        {
            var test = _fix.Create<Test>();
            var res = _sut.ToTestDto(test);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, test);
        }

        [Test]
        public void ToLecturerDto_From_Lecturer()
        {
            var lecturer = _fix.Create<Lecturer>();
            var res = _sut.ToLecturerDto(lecturer);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, lecturer);
        }

        [Test]
        public void ToSubjectFullDto_From_Subject()
        {
            var subject = _fix.Create<Subject>();
            var res = _sut.ToSubjectFullDto(subject);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, subject);
        }


        [Test]
        public void ToLecturer_From_LecturerRequest()
        {
            var request = _fix.Create<LecturerRequestDto>();
            var res = _sut.ToLecturer(request);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, request);
        }


        [Test]
        public void ToSchedule_From_ScheduleRequestDto()
        {
            var request = _fix.Create<ScheduleRequestDto>();
            var res = _sut.ToSchedule(request);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, request);
        }

        [Test]
        public void ToTest_From_TestRequestDto()
        {
            var request = _fix.Create<TestRequestDto>();
            var res = _sut.ToTest(request);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, request);
        }


        [Test]
        public void ToTestFullDto_From_Test()
        {
            var test = _fix.Create<Test>();
            var res = _sut.ToTestFullDto(test);
            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, test);
        }

    }
}
