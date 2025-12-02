using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces.Communication
{
    public interface ICommunicationMapper
    {
        ChatDto ToChatDto(Chat chat);
        Chat ToChat(ChatDto dto);
        ChatFullDto ToFullChatDto(Chat chat);

        MessageDto ToMessageDto(Message msg);
        Message ToMessage(MessageDto dto);
    }
}
