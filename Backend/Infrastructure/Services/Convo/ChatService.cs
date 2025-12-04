using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces.Convo;
using Core.Interfaces.Convo.FileServices;
using Core.Interfaces.LLM;

namespace Infrastructure.Services
{
    public class ChatService(
            IChatRepo repo,
            ILLMService LLMService,
            IChatMapper mapper,
            IFileProcessingService fileService
            ) : IChatService
    {
        private readonly IChatRepo _repo = repo;
        private readonly IChatMapper _mapper = mapper;
        private readonly ILLMService _LLMService = LLMService;
        private readonly IFileProcessingService _fileService = fileService;

        public async Task<List<ChatDto>> GetAllChatsPageAsync(
                int lastId,
                int pageSize)
            => (await _repo.GetAllChatsPageAsync(lastId, pageSize))
                .ConvertAll(_mapper.ToChatDto);

        public async Task<ChatFullDto> GetFullChatPageAsync(
                int chatId,
                int lastId,
                int pageSize)
        {
            var chat = await _repo.GetChatByIdAsync(chatId)
                ?? throw new ChatNotFoundException($"Chat With Id={chatId} Not Found");
            chat.Messages = await _repo.GetMessagesForChat(chatId, lastId, pageSize);
            return _mapper.ToFullChatDto(chat);
        }

        public async Task<MessageDto> DeleteMessageByIdAsync(int msgId)
            => _mapper.ToMessageDto(await _repo.DeleteMessageByIdAsync(msgId));


        public async Task<ChatDto> CreateNewChat(string chatName)
            => _mapper.ToChatDto(await _repo.CreateNewChat(chatName));


        public async Task<MessageDto> SendMessageAsync(MessageRequest messageRequest)
        {
            ClientMessage cm = messageRequest.Message;
            List<int> fileIds = messageRequest.FileIds;

            var fileDtos = (await Task.WhenAll(fileIds.Select(id => _repo.GetFileByIdAsync(id).AsTask())))
                .Where(f => f is not null)
                .Select(f => _mapper.ToFileDto(f!))
                .ToList();

            var request = _mapper.ToMessage(cm, fileDtos);

            await _repo.AddMessageAsync(request);
            var response = await _LLMService.SendMessageAsync(_mapper.ToMessageDto(cm, fileDtos));
            await _repo.AddMessageAsync(_mapper.ToMessage(response));

            return response;
        }

        public async Task<FileDto> RemoveFileAsync(int fileId)
            => _mapper.ToFileDto(await _repo.RemoveFileByIdAsync(fileId));

        public async Task<List<int>> UploadFilesForMessageAsync(List<FileUpload> fileUploads)
        {
            var fileDtos = await _fileService.ProcessFilesAsync(fileUploads);
            var files = fileDtos.ConvertAll(f => _mapper.ToChatFile(f));
            var onDbFiles = await _repo.AddFilesAsync(files);
            return onDbFiles.ConvertAll(f => f.Id);
        }
    }
}
