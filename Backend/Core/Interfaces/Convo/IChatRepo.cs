using Core.Entities;
using Core.Entities.Convo;

namespace Core.Interfaces.Convo
{
    public interface IChatRepo
    {
        ValueTask<Chat?> GetChatByIdAsync(int chatId);
        Task<List<Chat>> GetAllChatsPageAsync(int lastId, int pageSize);

        Task<List<Message>> GetMessagesForChat(int chatId, int lastId, int pageSize);
        Task<Message> AddMessageAsync(Message msg);
        Task<Message> DeleteMessageByIdAsync(int msgId);
    }
}
