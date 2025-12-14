using Core.Dtos;

namespace Core.Interfaces.Convo
{
    public interface IChatService
    {
        Task<List<ChatDto>> GetAllChatsPageAsync(int? lastId, int pageSize);
        Task<ChatDto> CreateNewChatAsync(CreateChatRequest request);

        Task<ChatFullDto> GetChatWithMessagesPageAsync(int chatId, int? lastId, int pageSize);

        Task<SendMessageResponseDto> SendMessageAsync(
                MessageRequest messageRequest,
                Action<string>? onRecieved);

        Task<List<int>> UploadFilesAsync(List<FileUpload> files);
        Task<FileDto> RemoveFileAsync(int fileId);
        Task<MessageDto> DeleteMessageByIdAsync(int msgId);
    }
}
