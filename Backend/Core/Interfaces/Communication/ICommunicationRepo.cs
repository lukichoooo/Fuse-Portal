using Core.Entities;

namespace Core.Interfaces
{
    public interface ICommunicationRepo
    {
        Task<Chat> GetChatAsync(int chatId);
        Task<Chat> GetChatWithMessagesPageAsync(int chatId, int lastId, int pageSize);
        Task<Chat> GetAllChatsPageAsync(int lastId, int pageSize);

        Task<Message> AddMessageAsync(Message msg);
        Task<Message> UpdateMessageAsync(Message msg);
        Task<Message> DeleteMessageAsync(Message msg);
    }
}
