using Core.Dtos;
using Core.Entities;
using Core.Entities.Convo;

namespace Core.Interfaces.Convo
{
    public interface IChatMapper
    {
        ChatDto ToChatDto(Chat chat);
        Chat ToChat(ChatDto dto);
        ChatFullDto ToFullChatDto(Chat chat);

        MessageDto ToMessageDto(Message msg);
        Message ToMessage(MessageDto dto);
    }
}
