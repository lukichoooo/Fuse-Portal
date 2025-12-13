using Core.Dtos;
using Core.Entities.Portal;

namespace Core.Interfaces.Portal
{
    public interface IPortalMapper
    {
        SubjectDto ToSubjectDto(Subject subject);
        SubjectFullDto ToSubjectFullDto(Subject subject);
        Subject ToSubject(SubjectRequestDto dto, int userId);

        SyllabusDto ToSyllabusDto(Syllabus syllabus);

        LecturerDto ToLecturerDto(Lecturer lecturer);

        Schedule ToSchedule(ScheduleDto dto);
        ScheduleDto ToScheduleDto(Schedule schedule);


        // Requests

        Lecturer ToLecturer(LecturerRequestDto dto);
        Schedule ToSchedule(ScheduleRequestDto dto);
        Syllabus ToSyllabus(SyllabusRequestDto dto);
        SyllabusFullDto ToSyllabusFullDto(Syllabus syllabus);

        Subject ToSubjectWithoutLists(SubjectFullRequestDto fullRequest, int userId);

        Schedule ToSchedule(ScheduleRequestDtoNoSubjectId dto, int subjectId);
        Lecturer ToLecturer(LecturerRequestDtoNoSubjectId dto, int subjectId);
        Syllabus ToSyllabus(SyllabusRequestDtoNoSubjectId dto, int subjectId);

        Exam ToExam(ExamDto dto);
        ExamDto ToExamDto(Exam exam);
    }
}
