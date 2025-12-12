using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces.Auth;
using Core.Interfaces.Portal;

namespace Infrastructure.Services.Portal
{
    public class PortalService(
            IPortalRepo repo,
            IPortalMapper mapper,
            ICurrentContext currentContext,
            IPortalParser portalParser,
            IMockExamCreator examCreator
            ) : IPortalService
    {
        private readonly IPortalRepo _repo = repo;
        private readonly IPortalMapper _mapper = mapper;
        private readonly ICurrentContext _currentContext = currentContext;
        private readonly IPortalParser _portalParser = portalParser;
        private readonly IMockExamCreator _examCreator = examCreator;


        // TODO: Write tests / implement functionality
        public async Task<MockExamResponse> GenerateMockExamForSyllabus(int syllabusId)
        {
            var userId = _currentContext.GetCurrentUserId();
            var syllabus = await _repo.GetFullSyllabusByIdAsync(syllabusId, userId);
            var examContent = await _examCreator.GenerateExamAsync(syllabus.Content);
            return new MockExamResponse(examContent);
        }

        public async Task<PortalParserResponseDto> ParseAndSavePortalAsync(string HtmlPage)
        {
            PortalParserResponseDto portal = await _portalParser.ParsePortalHtml(HtmlPage);

            foreach (var subjectFullRequest in portal.Subjects)
            {
                int userId = _currentContext.GetCurrentUserId();
                var subject = _mapper.ToSubjectWithoutLists(
                        subjectFullRequest,
                        userId);
                var onDbSubject = await _repo.AddSubjectForUserAsync(subject);
                int subjectId = onDbSubject.Id;

                foreach (var scheduleNoSubjectId in subjectFullRequest.Schedules)
                {
                    var schedule = _mapper.ToSchedule(scheduleNoSubjectId, subjectId);
                    await _repo.AddScheduleForSubjectAsync(schedule, userId);
                }

                foreach (var lecturerNoSubjectId in subjectFullRequest.Lecturers)
                {
                    var lecturer = _mapper.ToLecturer(lecturerNoSubjectId, subjectId);
                    await _repo.AddLecturerToSubjectAsync(lecturer, userId);
                }

                foreach (var sylabusNoSubjectId in subjectFullRequest.Syllabuses)
                {
                    var sylabus = _mapper.ToSyllabus(sylabusNoSubjectId, subjectId);
                    await _repo.AddSyllabusForSubjectAsync(sylabus, userId);
                }
            }
            return portal;
        }

        public async Task<SubjectFullDto> AddSubjectForCurrentUserAsync(SubjectRequestDto dto)
        {
            int userId = _currentContext.GetCurrentUserId();
            var subject = _mapper.ToSubject(dto, userId);
            return _mapper.ToSubjectFullDto(await _repo.AddSubjectForUserAsync(subject));
        }

        public async Task<SubjectDto> RemoveSubjectByIdAsync(int subjectId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return _mapper.ToSubjectDto(await _repo.RemoveSubjectById(subjectId, userId));
        }

        public async Task<List<SubjectDto>> GetSubjectsPageForCurrentUserAsync(
                int? lastSubjectId,
                int pageSize)
        {
            int userId = _currentContext.GetCurrentUserId();
            return (await _repo.GetSubjectsPageForUserAsync(userId, lastSubjectId, pageSize))
                .ConvertAll(_mapper.ToSubjectDto);
        }

        public async Task<SubjectFullDto> GetFullSubjectByIdAsync(int subjectId)
        {
            int userId = _currentContext.GetCurrentUserId();
            var subject = await _repo.GetFullSubjectByIdAsync(subjectId, userId)
                ?? throw new SubjectNotFoundException(
                        $" subject Not Found With Id={subjectId} and UserId={userId}");
            return _mapper.ToSubjectFullDto(subject);
        }

        public async Task<ScheduleDto> AddScheduleForSubjectAsync(ScheduleRequestDto request)
        {
            int userId = _currentContext.GetCurrentUserId();
            var schedule = _mapper.ToSchedule(request);
            return _mapper.ToScheduleDto(await _repo.AddScheduleForSubjectAsync(
                        schedule, userId));
        }

        public async Task<ScheduleDto> RemoveScheduleByIdAsync(int scheduleId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return _mapper.ToScheduleDto(await _repo.RemoveScheduleByIdAsync(
                        scheduleId, userId));
        }

        public async Task<LecturerDto> AddLecturerToSubjectAsync(LecturerRequestDto request)
        {
            int userId = _currentContext.GetCurrentUserId();
            var lecturer = _mapper.ToLecturer(request);
            return _mapper.ToLecturerDto(await _repo.AddLecturerToSubjectAsync(
                        lecturer, userId));
        }

        public async Task<LecturerDto> RemoveLecturerByIdAsync(int lecturerId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return _mapper.ToLecturerDto(await _repo.RemoveLecturerByIdAsync(
                                    lecturerId, userId));
        }

        public async Task<SyllabusDto> AddSylabusForSubjectAsync(SyllabusRequestDto request)
        {
            int userId = _currentContext.GetCurrentUserId();
            var sylabus = _mapper.ToSyllabus(request);
            return _mapper.ToSyllabusDto(await _repo.AddSyllabusForSubjectAsync(
                        sylabus, userId));
        }

        public async Task<SyllabusDto> RemoveSyllabusByIdAsync(int syllabiId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return _mapper.ToSyllabusDto(await _repo.RemoveSyllabusByIdAsync(
                        syllabiId, userId));
        }

        public async Task<SyllabusFullDto> GetFullSyllabusByIdAsync(int syllabiId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return _mapper.ToSyllabusFullDto(await _repo.GetFullSyllabusByIdAsync(
                        syllabiId, userId));
        }
    }
}
