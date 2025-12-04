using Core.Dtos;
using Core.Entities.Convo;

namespace Core.Interfaces.Convo
{
    public interface IChatMapper
    {
        ChatDto ToChatDto(Chat chat);
        Chat ToChat(ChatDto dto);
        ChatFullDto ToFullChatDto(Chat chat);

        ChatFile ToChatFile(FileDto dto, int? messageId = null);
        FileDto ToFileDto(ChatFile f);

        MessageDto ToMessageDto(Message msg);
        MessageDto ToMessageDto(ClientMessage cm, List<FileDto>? files = null);
        Message ToMessage(ClientMessage cm, List<FileDto>? files = null);
        Message ToMessage(MessageDto dto);
    }
}
