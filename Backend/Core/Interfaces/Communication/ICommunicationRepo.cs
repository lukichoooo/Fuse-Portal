using Core.Entities;

namespace Core.Interfaces
{
    public interface ICommunicationRepo
    {
        ValueTask<Chat?> GetChatAsync(int chatId);
        Task<List<Chat>> GetAllChatsPageAsync(int lastId, int pageSize);

        Task<List<Message>> GetMessagesForChat(int chatId, int lastId, int pageSize);
        Task<Message> AddMessageAsync(Message msg);
        Task<Message> DeleteMessageByIdAsync(int msgId);
    }
}
