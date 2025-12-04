using Core.Dtos;

namespace Core.Interfaces.Convo
{
    public interface IChatService
    {
        Task<List<ChatDto>> GetAllChatsPageAsync(int lastId, int pageSize);
        Task<ChatDto> CreateNewChat(string chatName);

        Task<ChatFullDto> GetFullChatPageAsync(int chatId, int lastId, int pageSize);

        Task<MessageDto> SendMessageAsync(MessageRequest messageRequest);
        Task<List<int>> UploadFilesForMessageAsync(List<FileUpload> files);
        Task<FileDto> RemoveFileAsync(int fileId);
        Task<MessageDto> DeleteMessageByIdAsync(int msgId);
    }
}
