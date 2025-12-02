using Core.Dtos;

namespace Core.Interfaces.Convo
{
    public interface IChatService
    {
        Task<List<ChatDto>> GetAllChatsPageAsync(int lastId, int pageSize);

        Task<ChatFullDto> GetFullChatPageAsync(int chatId, int lastId, int pageSize);

        Task<MessageDto> SendMessageAsync(MessageDto msg);
        Task<MessageDto> RemoveMessageByIdAsync(int msgId);
    }
}
