using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces.Convo;
using Core.Interfaces.LLM;

namespace Infrastructure.Services
{
    public class ChatService(
            IChatRepo repo,
            ILLMService LLMService,
            IChatMapper mapper) : IChatService
    {
        private readonly IChatRepo _repo = repo;
        private readonly IChatMapper _mapper = mapper;
        private readonly ILLMService _LLMService = LLMService;

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

        // TODO: ADD file handling
        // TODO: Make Concurrent (maybe add fire & forget)
        public async Task<MessageDto> SendMessageAsync(ClientMessage cm)
        {
            await _repo.AddMessageAsync(_mapper.ToMessage(cm));
            MessageDto response = await _LLMService.SendMessageAsync(_mapper.ToMessageDto(cm));
            await _repo.AddMessageAsync(_mapper.ToMessage(response));

            return response;
        }

    }
}
