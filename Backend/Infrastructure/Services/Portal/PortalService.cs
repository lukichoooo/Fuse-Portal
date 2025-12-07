using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces.Auth;
using Core.Interfaces.Portal;

namespace Infrastructure.Services.Portal
{
    public class PortalService(
            IPortalRepo repo,
            IPortalMapper mapper,
            ICurrentContext currentContext
            ) : IPortalService
    {
        private readonly IPortalRepo _repo = repo;
        private readonly IPortalMapper _mapper = mapper;
        private readonly ICurrentContext _currentContext = currentContext;

        public async Task<SubjectDto> AddSubjectForCurrentUser(SubjectDto dto)
        {
            int userId = _currentContext.GetCurrentUserId();
            var subject = _mapper.ToSubject(dto, userId);
            return _mapper.ToSubjectDto(await _repo.AddSubjectForUser(subject));
        }

        public async Task<SubjectDto> RemoveSubjectById(int subjectId)
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

        public async Task<SubjectFullDto> GetFullSubjectById(int subjectId)
        {
            int userId = _currentContext.GetCurrentUserId();
            var subject = await _repo.GetFullSubjectById(subjectId, userId)
                ?? throw new SubjectNotFoundException(
                        $" subject Not Found With Id={subjectId} and UserId={userId}");
            return _mapper.ToSubjectFullDto(subject);
        }
    }
}
