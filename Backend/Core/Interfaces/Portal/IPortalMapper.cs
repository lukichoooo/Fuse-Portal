using Core.Dtos;
using Core.Entities.Portal;

namespace Core.Interfaces.Portal
{
    public interface IPortalMapper
    {
        SubjectDto ToSubjectDto(Subject subject);
        SubjectFullDto ToSubjectFullDto(Subject subject);
        Subject ToSubject(SubjectDto dto, int userId);

        TestDto ToTestDto(Test test);

        LecturerDto ToLecturerDto(Lecturer lecturer);

        Schedule ToSchedule(ScheduleDto dto);
        ScheduleDto ToScheduleDto(Schedule schedule);
    }
}
