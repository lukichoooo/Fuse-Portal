using Core.Dtos;
using Core.Entities.Portal;
using Core.Interfaces.Portal;

namespace Infrastructure.Services.Portal
{
    public class PortalMapper : IPortalMapper // TODO: Write tests
    {
        public Course ToCourse(CourseDto dto)
            => new()
            {
                Id = dto.Id,
                Name = dto.Name,
                MetaData = dto.Metadata
            };

        public CourseDto ToCourseDto(Course course)
            => new(
                    Id: course.Id,
                    Name: course.Name,
                    Metadata: course.MetaData
                  );

        public Schedule ToSchedule(ScheduleDto dto)
            => new()
            {
                Id = dto.Id,
                Start = dto.Start,
                End = dto.End,
                MetaData = dto.Metadata
            };

        public ScheduleDto ToScheduleDto(Schedule schedule)
            => new(
                    Id: schedule.Id,
                    Start: schedule.Start,
                    End: schedule.End,
                    Metadata: schedule.MetaData
                  );

        public Subject ToSubject(SubjectDto dto)
            => new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Metadata = dto.Metadata
            };

        public SubjectDto ToSubjectDto(Subject subject)
            => new(
                    Id: subject.Id,
                    Name: subject.Name,
                    Metadata: subject.Metadata
                  );

        public Test ToTest(TestDto dto)
            => new()
            {
                Id = dto.Id,
                Name = dto.Name
            };

        public TestDto ToTestDto(Test test)
            => new(
                    Id: test.Id,
                    Name: test.Name
                  );
    }
}
