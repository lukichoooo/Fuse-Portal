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

        public Subject ToSubject(SubjectDto dto, int userId)
            => new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Metadata = dto.Metadata,
                UserId = userId
            };

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

        public TestDto ToTestDto(Test test)
            => new(
                    Id: test.Id,
                    Name: test.Name
                  );
    }
}
