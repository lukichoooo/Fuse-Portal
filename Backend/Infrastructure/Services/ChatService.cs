using Core.Dtos;
using Core.Interfaces.Convo;

namespace Infrastructure.Services
{
    public class ChatService(IChatRepo repo, IChatMapper mapper) : IChatService
    {
        private readonly IChatRepo _repo = repo;
        private readonly IChatMapper _mapper = mapper;

        public async Task<List<ChatDto>> GetAllChatsPageAsync(int lastId, int pageSize)
            => (await _repo.GetAllChatsPageAsync(lastId, pageSize))
                .ConvertAll(_mapper.ToChatDto);

        public Task<ChatFullDto> GetFullChatPageAsync(int chatId, int lastId, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<MessageDto> RemoveMessageByIdAsync(int msgId)
        {
            throw new NotImplementedException();
        }

        public Task<MessageDto> SendMessageAsync(MessageDto msg)
        {
            throw new NotImplementedException();
        }
    }
}
