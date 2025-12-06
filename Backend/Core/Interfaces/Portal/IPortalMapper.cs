using Core.Dtos;
using Core.Entities.Portal;

namespace Core.Interfaces.Portal
{
    public interface IPortalMapper
    {
        SubjectDto ToSubjectDto(Subject subject);
        Subject ToSubject(SubjectDto dto);

        TestDto ToTestDto(Test test);

        Course ToCourse(CourseDto dto);
        CourseDto ToCourseDto(Course course);

        Schedule ToSchedule(ScheduleDto dto);
        ScheduleDto ToScheduleDto(Schedule schedule);
    }
}
