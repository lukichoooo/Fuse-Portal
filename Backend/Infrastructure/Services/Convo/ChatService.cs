using Core.Dtos;
using Core.Entities.Convo;
using Core.Interfaces.Auth;
using Core.Interfaces.Convo;
using Core.Interfaces.Convo.FileServices;
using Core.Interfaces.LLM;

namespace Infrastructure.Services
{
    public class ChatService(
            IChatRepo repo,
            ILLMMessageService LLMService,
            IChatMapper mapper,
            IFileProcessingService fileService,
            ICurrentContext currentContext
            ) : IChatService
    {
        private readonly IChatRepo _repo = repo;
        private readonly IChatMapper _mapper = mapper;
        private readonly ILLMMessageService _LLMService = LLMService;
        private readonly IFileProcessingService _fileService = fileService;
        private readonly ICurrentContext _currentContext = currentContext;

        public async Task<List<ChatDto>> GetAllChatsPageAsync(
                int? lastId,
                int pageSize)
        {
            int userId = _currentContext.GetCurrentUserId();
            return (await _repo.GetAllChatsForUserPageAsync(lastId, pageSize, userId))
                .ConvertAll(_mapper.ToChatDto);
        }

        public async Task<ChatFullDto> GetChatWithMessagesPageAsync(
                int chatId,
                int? firstMsgId,
                int pageSize)
        {
            int userId = _currentContext.GetCurrentUserId();
            var chat = await _repo.GetChatWithMessagesPageAsync(
                    chatId, firstMsgId, pageSize, userId);
            return _mapper.ToFullChatDto(chat);
        }

        public async Task<MessageDto> DeleteMessageByIdAsync(int msgId)
        {
            int userId = _currentContext.GetCurrentUserId();
            return _mapper.ToMessageDto(await _repo.DeleteMessageByIdAsync(msgId, userId));
        }


        public async Task<ChatDto> CreateNewChatAsync(CreateChatRequest request)
        {
            int userId = _currentContext.GetCurrentUserId();
            var chat = new Chat
            {
                UserId = userId,
                Name = request.ChatName,
            };
            return _mapper.ToChatDto(await _repo.CreateNewChatAsync(chat));
        }

        public async Task<SendMessageResponseDto> SendMessageAsync(
            MessageRequest messageRequest,
            Func<string, Task>? onReceived)
        {
            int userId = _currentContext.GetCurrentUserId();

            var fileDtos = await GetFileDtosAsync(messageRequest.FileIds, userId);

            var userMessage = _mapper.ToMessage(messageRequest.Message, userId, fileDtos);
            var userMessageDb = await _repo.AddMessageAsync(userMessage);
            var messageDto = _mapper.ToMessageDto(messageRequest.Message, fileDtos);

            var llmResponse = await _LLMService.SendMessageAsync(messageDto, onReceived);

            var responseDb = await _repo.AddMessageAsync(_mapper.ToMessage(llmResponse, userId));

            return new SendMessageResponseDto
            {
                UserMessage = _mapper.ToMessageDto(userMessageDb),
                Response = _mapper.ToMessageDto(responseDb)
            };
        }


        public async Task<FileDto> RemoveFileAsync(int fileId)
            => _mapper.ToFileDto(await _repo.RemoveFileByIdAsync(
                        fileId,
                        _currentContext.GetCurrentUserId()));

        public async Task<List<int>> UploadFilesAsync(List<FileUpload> fileUploads)
        {
            int userId = _currentContext.GetCurrentUserId();
            var fileDtos = await _fileService.ProcessFilesAsync(fileUploads);
            var files = fileDtos.ConvertAll(f => _mapper.ToChatFile(f, userId));
            var onDbFiles = await _repo.AddFilesAsync(files);
            return onDbFiles.ConvertAll(f => f.Id);
        }


        // Helper
        private async Task<List<FileDto>> GetFileDtosAsync(IEnumerable<int> fileIds, int userId)
        {
            var files = await Task.WhenAll(fileIds
                .Select(id => _repo.GetFileByIdAsync(id, userId).AsTask()));
            return files
                .Where(f => f != null)
                .Select(f => _mapper.ToFileDto(f!))
                .ToList();
        }

    }
}
