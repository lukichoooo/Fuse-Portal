using Core.Entities.Convo;

namespace Core.Interfaces.Convo
{
    public interface IChatRepo
    {
        ValueTask<Chat?> GetChatByIdAsync(int chatId);
        Task<Chat> CreateNewChat(string chatName);
        Task<List<Chat>> GetAllChatsPageAsync(int lastId, int pageSize);
        Task<Chat> UpdateChatLastResponseIdAsync(int chatId, string newResponseId);

        Task<List<Message>> GetMessagesForChat(int chatId, int lastId, int pageSize);
        Task<Message> AddMessageAsync(Message msg);
        Task<Message> DeleteMessageByIdAsync(int msgId);

        ValueTask<ChatFile?> GetFileByIdAsync(int fileId);
        Task<List<ChatFile>> AddFilesAsync(List<ChatFile> files);
        Task<ChatFile> RemoveFileByIdAsync(int fileId);
    }
}
