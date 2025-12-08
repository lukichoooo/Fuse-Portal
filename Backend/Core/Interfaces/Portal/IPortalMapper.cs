using Core.Dtos;
using Core.Entities.Portal;

namespace Core.Interfaces.Portal
{
    public interface IPortalMapper
    {
        SubjectDto ToSubjectDto(Subject subject);
        SubjectFullDto ToSubjectFullDto(Subject subject);
        Subject ToSubject(SubjectRequestDto dto, int userId);

        TestDto ToTestDto(Test test);

        LecturerDto ToLecturerDto(Lecturer lecturer);

        Schedule ToSchedule(ScheduleDto dto);
        ScheduleDto ToScheduleDto(Schedule schedule);


        // Requests

        Lecturer ToLecturer(LecturerRequestDto dto);
        Schedule ToSchedule(ScheduleRequestDto dto);
        Test ToTest(TestRequestDto dto);
        TestFullDto ToTestFullDto(Test test);

        Subject ToSubjectWithoutLists(SubjectFullRequestDto fullRequest, int userId);

        Schedule ToSchedule(ScheduleRequestDtoNoSubjectId dto, int subjectId);
        Lecturer ToLecturer(LecturerRequestDtoNoSubjectId dto, int subjectId);
        Test ToTest(TestRequestDtoNoSubjectId dto, int subjectId);
    }
}
