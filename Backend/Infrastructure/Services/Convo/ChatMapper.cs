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
                Files = dto.Files.ConvertAll(x => new ChatFile
                {
                    Name = x.Name,
                    Text = x.Text,
                    MessageId = dto.Id
                })
            };



        public MessageDto ToMessageDto(Message msg)
            => new()
            {
                Id = msg.Id,
                Text = msg.Text,
                CreatedAt = msg.CreatedAt,
                ChatId = msg.ChatId,
                Files = msg.Files.ConvertAll(x => new FileDto(x.Name, x.Text))
            };



        // From Client

        public Message ToMessage(ClientMessage cm)
            => new()
            {
                Text = cm.Text,
                ChatId = cm.ChatId,
            };

        public MessageDto ToMessageDto(ClientMessage cm, List<FileDto>? files = null)
            => new()
            {
                Text = cm.Text,
                ChatId = cm.ChatId,
                Files = files ?? []
            };
    }
}
