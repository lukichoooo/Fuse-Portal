using Core.Dtos;

namespace Core.Interfaces
{
    public interface ICommunicationService
    {
        Task<List<ChatDto>> GetAllChatsPageAsync(int lastId, int pageSize);

        Task<ChatFullDto> GetFullChatPageAsync(int chatId, int lastId, int pageSize);

        Task<MessageDto> SendMessageAsync(MessageDto msgDto);
        Task<MessageDto> SendMessageWithFileAsync(MessageWithFileDto msgDto);
        Task<MessageDto> RemoveMessageByIdAsync(int msgId);
    }
}
