using Core.Dtos;
using Core.Entities.Convo;
using Core.Exceptions;
using Core.Interfaces.Auth;
using Core.Interfaces.Convo;
using Core.Interfaces.Convo.FileServices;
using Core.Interfaces.LLM;

namespace Infrastructure.Services
{
    public class ChatService(
            IChatRepo repo,
            ILLMService LLMService,
            IChatMapper mapper,
            IFileProcessingService fileService,
            ICurrentContext currentContext
            ) : IChatService
    {
        private readonly IChatRepo _repo = repo;
        private readonly IChatMapper _mapper = mapper;
        private readonly ILLMService _LLMService = LLMService;
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
                int? lastId,
                int pageSize)
        {
            int userId = _currentContext.GetCurrentUserId();
            var chat = await _repo.GetChatWithMessagesPageAsync(
                    chatId, lastId, pageSize, userId);
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


        public async Task<MessageDto> SendMessageAsync(MessageRequest messageRequest)
        {
            ClientMessage clientMessage = messageRequest.Message;
            List<int> fileIds = messageRequest.FileIds;
            int userId = _currentContext.GetCurrentUserId();

            var fileDtos = (await Task.WhenAll(fileIds
                        .Select(id => _repo.GetFileByIdAsync(id, userId).AsTask())
                        ))
                .Where(f => f is not null)
                .Select(f => _mapper.ToFileDto(f!))
                .ToList();

            var message = _mapper.ToMessage(clientMessage, userId, fileDtos);

            await _repo.AddMessageAsync(message);
            foreach (var fileId in fileIds)
            {
                await _repo.AddStoredFileToMessage(message.Id, fileId, userId);
            }
            var response = await _LLMService
                .SendMessageAsync(_mapper.ToMessageDto(clientMessage, fileDtos));
            await _repo.AddMessageAsync(_mapper.ToMessage(response, userId));

            return response;
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
    }
}
