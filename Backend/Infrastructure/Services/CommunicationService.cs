using Core.Dtos;
using Core.Interfaces;
using Core.Interfaces.Communication;

namespace Infrastructure.Services
{
    public class CommunicationService(ICommunicationRepo repo, ICommunicationMapper mapper) : ICommunicationService
    {
        private readonly ICommunicationRepo _repo = repo;
        private readonly ICommunicationMapper _mapper = mapper;

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
