using Core.Dtos;
using Core.Entities.Convo;

namespace Core.Interfaces.Convo
{
    public interface IChatMapper
    {
        ChatDto ToChatDto(Chat chat);
        Chat ToChat(ChatDto dto, int userId);
        ChatFullDto ToFullChatDto(Chat chat);

        ChatFile ToChatFile(FileDto dto, int userId, int? messageId = null);
        FileDto ToFileDto(ChatFile f);

        MessageDto ToMessageDto(Message msg);
        MessageDto ToMessageDto(ClientMessage cm, List<FileDto>? files = null);
        Message ToMessage(ClientMessage cm, int userId, List<FileDto>? files = null);
        Message ToMessage(MessageDto dto, int userId);
    }
}
