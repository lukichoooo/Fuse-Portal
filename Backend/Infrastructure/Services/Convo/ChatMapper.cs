using Core.Dtos;
using Core.Entities.Convo;
using Core.Interfaces.Convo;

namespace Infrastructure.Services
{
    public class ChatMapper : IChatMapper
    {
        public Chat ToChat(ChatDto dto)
            => new()
            {
                Id = dto.Id,
                Name = dto.Name,
            };

        public ChatDto ToChatDto(Chat chat)
            => new(
                    Id: chat.Id,
                    Name: chat.Name
                  );

        public ChatFullDto ToFullChatDto(Chat chat)
            => new(
                    Id: chat.Id,
                    Name: chat.Name,
                    Messages: chat.Messages.ConvertAll(ToMessageDto)
                  );

        public Message ToMessage(MessageDto dto)
            => new()
            {
                Id = dto.Id,
                Text = dto.Text,
                CreatedAt = dto.CreatedAt,
                ChatId = dto.ChatId,
                Files = dto.Files.ConvertAll(f => ToChatFile(f))
            };



        public MessageDto ToMessageDto(Message msg)
            => new()
            {
                Id = msg.Id,
                Text = msg.Text,
                CreatedAt = msg.CreatedAt,
                ChatId = msg.ChatId,
                Files = msg.Files.ConvertAll(ToFileDto)
            };



        // From Client

        public Message ToMessage(ClientMessage cm, List<FileDto>? files = null)
            => new()
            {
                Text = cm.Text,
                ChatId = cm.ChatId,
                Files = files?.ConvertAll(f => ToChatFile(f)) ?? []
            };

        public MessageDto ToMessageDto(ClientMessage cm, List<FileDto>? files = null)
            => new()
            {
                Text = cm.Text,
                ChatId = cm.ChatId,
                Files = files ?? []
            };

        public ChatFile ToChatFile(FileDto dto, int? messageId = null)
            => new()
            {
                Text = dto.Text,
                Name = dto.Name,
                MessageId = messageId
            };


        public FileDto ToFileDto(ChatFile f)
            => new(
                    Name: f.Name,
                    Text: f.Text
                    );

    }
}
