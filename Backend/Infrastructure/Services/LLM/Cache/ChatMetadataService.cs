using Core.Interfaces.Convo;
using Core.Interfaces.LLM.Cache;

namespace Infrastructure.Services.LLM.Cache
{
    public class ChatMetadataService(
            IChatRepo repo,
            IChatMetadataCache cache
            ) : IChatMetadataService
    {
        private readonly IChatMetadataCache _cache = cache;
        private readonly IChatRepo _chatRepo = repo;

        public async Task<string?> GetLastResponseIdAsync(int chatId)
            => await _cache.GetValueAsync(chatId)
                ?? (await _chatRepo.GetChatByIdAsync(chatId))?.LastResponseId;

        // TODO: Make Concurrent
        public async Task SetLastResponseIdAsync(int chatId, string responseId)
        {
            await _chatRepo.UpdateChatLastResponseIdAsync(chatId, responseId);
            await _cache.SetValueAsync(chatId, responseId);
        }
    }
}
