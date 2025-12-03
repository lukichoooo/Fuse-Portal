using Core.Dtos;
using Core.Entities.Convo;

namespace Core.Interfaces.Convo
{
    public interface IChatMapper
    {
        ChatDto ToChatDto(Chat chat);
        Chat ToChat(ChatDto dto);
        ChatFullDto ToFullChatDto(Chat chat);

        MessageDto ToMessageDto(Message msg);
        MessageDto ToMessageDto(ClientMessage cm, List<FileDto>? files = null);
        Message ToMessage(ClientMessage cm);
        Message ToMessage(MessageDto dto);
    }
}
