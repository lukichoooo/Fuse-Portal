using Core.Dtos;
using Core.Entities.Portal;
using Core.Interfaces.Portal;

namespace Infrastructure.Services.Portal
{
    public class PortalMapper : IPortalMapper
    {

        public Schedule ToSchedule(ScheduleDto dto)
            => new()
            {
                Id = dto.Id,
                Start = dto.Start,
                End = dto.End,
                MetaData = dto.Metadata,
                SubjectId = dto.SubjectId
            };

        public ScheduleDto ToScheduleDto(Schedule schedule)
            => new(
                    Id: schedule.Id,
                    Start: schedule.Start,
                    End: schedule.End,
                    SubjectId: schedule.SubjectId,
                    Metadata: schedule.MetaData
                );

        public LecturerDto ToLecturerDto(Lecturer lecturer)
            => new(
                    Id: lecturer.Id,
                    Name: lecturer.Name
                  );

        public SubjectDto ToSubjectDto(Subject subject)
            => new(
                    Id: subject.Id,
                    Name: subject.Name,
                    Metadata: subject.Metadata
                  );

        public SubjectFullDto ToSubjectFullDto(Subject subject)
            => new()
            {
                Id = subject.Id,
                Name = subject.Name,
                Grade = subject.Grade,
                Metadata = subject.Metadata,
                Lecturers = subject.Lecturers
                    .ConvertAll(ToLecturerDto),
                Schedules = subject.Schedules
                    .ConvertAll(ToScheduleDto),
                Tests = subject.Tests
                    .ConvertAll(ToTestDto)
            };

        public Subject ToSubject(SubjectRequestDto dto, int userId)
            => new()
            {
                Name = dto.Name,
                UserId = userId,
                Metadata = dto.Metadata,
                Grade = dto.Grade
            };

        public Lecturer ToLecturer(LecturerRequestDto dto)
            => new()
            {
                Name = dto.Name,
                SubjectId = dto.SubjectId,
            };

        public Schedule ToSchedule(ScheduleRequestDto dto)
            => new()
            {
                SubjectId = dto.SubjectId,
                Start = dto.Start,
                End = dto.End,
                Location = dto.Location,
                MetaData = dto.Metadata
            };

        public TestDto ToTestDto(Test test)
            => new(
                    Id: test.Id,
                    Name: test.Name
                  );

        public Test ToTest(TestRequestDto dto)
            => new()
            {
                Content = dto.Content,
                Name = dto.Name,
                SubjectId = dto.SubjectId,
                Date = dto.Date,
                Metadata = dto.Metadata
            };

        public TestFullDto ToTestFullDto(Test test)
            => new(
                    Id: test.Id,
                    Name: test.Name,
                    Content: test.Content,
                    Date: test.Date,
                    Metadata: test.Metadata
                  );
    }
}
