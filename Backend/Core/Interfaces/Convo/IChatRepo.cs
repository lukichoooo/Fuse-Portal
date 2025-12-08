using Core.Entities.Convo;

namespace Core.Interfaces.Convo
{
    public interface IChatRepo
    {
        ValueTask<Chat?> GetChatByIdAsync(int chatId, int userId);
        Task<Chat> CreateNewChatAsync(Chat chat);
        Task<List<Chat>> GetAllChatsForUserPageAsync(int? lastId, int pageSize, int userId);
        Task<Chat> UpdateChatLastResponseIdAsync(int chatId, string newResponseId, int userId);

        Task<Chat> GetChatWithMessagesPageAsync(int chatId, int? lastId, int pageSize, int userId);
        Task<Message> AddMessageAsync(Message msg);
        Task<Message> DeleteMessageByIdAsync(int msgId, int userId);

        ValueTask<ChatFile?> GetFileByIdAsync(int fileId, int userId);
        Task<List<ChatFile>> AddFilesAsync(List<ChatFile> files);
        Task<ChatFile> RemoveFileByIdAsync(int fileId, int userId);
        Task<ChatFile> AddStoredFileToMessage(int fileId, int messageId, int userId);
    }

}
