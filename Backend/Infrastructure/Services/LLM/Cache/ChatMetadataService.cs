using Core.Interfaces.Auth;
using Core.Interfaces.Convo;
using Core.Interfaces.LLM.Cache;

namespace Infrastructure.Services.LLM.Cache
{
    public class ChatMetadataService(
            IChatRepo repo,
            IChatMetadataCache cache,
            ICurrentContext currentContext
            ) : IChatMetadataService
    {
        private readonly IChatMetadataCache _cache = cache;
        private readonly IChatRepo _chatRepo = repo;
        private readonly ICurrentContext _currentContext = currentContext;

        public async Task<string?> GetLastResponseIdAsync(int chatId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return await _cache.GetValueAsync(chatId)
                ?? (await _chatRepo.GetChatByIdAsync(chatId, userId))?.LastResponseId;
        }

        public Task SetLastResponseIdAsync(int chatId, string responseId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return Task.WhenAll
            (
                _chatRepo.UpdateChatLastResponseIdAsync(chatId, responseId, userId),
                _cache.SetValueAsync(chatId, responseId)
            );
        }
    }
}
