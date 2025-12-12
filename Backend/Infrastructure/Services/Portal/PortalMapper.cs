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
                Metadata = dto.Metadata,
                SubjectId = dto.SubjectId
            };

        public ScheduleDto ToScheduleDto(Schedule schedule)
            => new(
                    Id: schedule.Id,
                    Start: schedule.Start,
                    End: schedule.End,
                    SubjectId: schedule.SubjectId,
                    Metadata: schedule.Metadata
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
                    Schedules: subject.Schedules.ConvertAll(ToScheduleDto),
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
                Syllabuses = subject.Syllabuses
                    .ConvertAll(ToSyllabusDto)
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
                Metadata = dto.Metadata
            };

        public SyllabusDto ToSyllabusDto(Syllabus sylabus)
            => new(
                    Id: sylabus.Id,
                    Name: sylabus.Name
                  );

        public Syllabus ToSyllabus(SyllabusRequestDto dto)
            => new()
            {
                Content = dto.Content,
                Name = dto.Name,
                SubjectId = dto.SubjectId,
                Metadata = dto.Metadata
            };

        public SyllabusFullDto ToSyllabusFullDto(Syllabus sylabus)
            => new(
                    Id: sylabus.Id,
                    Name: sylabus.Name,
                    Content: sylabus.Content,
                    Metadata: sylabus.Metadata
                  );

        public Subject ToSubjectWithoutLists(SubjectFullRequestDto fullRequest, int userId)
            => new()
            {
                Name = fullRequest.Name,
                Metadata = fullRequest.Metadata,
                Grade = fullRequest.Grade,
                UserId = userId,
            };


        public Schedule ToSchedule(
                ScheduleRequestDtoNoSubjectId dto,
                int subjectId)
            => new()
            {
                Start = dto.Start,
                End = dto.End,
                Metadata = dto.Metadata,
                Location = dto.Location,
                SubjectId = subjectId
            };


        public Lecturer ToLecturer(
                LecturerRequestDtoNoSubjectId dto,
                int subjectId)
            => new()
            {
                Name = dto.Name,
                SubjectId = subjectId
            };


        public Syllabus ToSyllabus(
                SyllabusRequestDtoNoSubjectId dto,
                int subjectId)
            => new()
            {
                Name = dto.Name,
                Content = dto.Content,
                Metadata = dto.Metadata,
                SubjectId = subjectId
            };
    }
}
