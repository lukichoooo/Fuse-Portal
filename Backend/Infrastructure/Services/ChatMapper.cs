using Core.Dtos;
using Core.Entities;
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
                    Messages: chat.Messages
                        .ConvertAll(ToMessageDto)
                  );

        public Message ToMessage(MessageDto dto)
            => new()
            {
                Id = dto.Id,
                Text = dto.Text,
                CreatedAt = dto.CreatedAt,
                ChatId = dto.ChatId,
                Files = dto.FileToContent?
                    .Select(x => new ChatFile
                    {
                        Name = x.Key,
                        Text = x.Value
                    }).ToList() ?? []
            };

        public MessageDto ToMessageDto(Message msg)
            => new(
                        Id: msg.Id,
                        Text: msg.Text,
                        CreatedAt: msg.CreatedAt,
                        ChatId: msg.ChatId,
                        FileToContent:
                            msg.Files.Count != 0 ?
                                msg.Files.ToDictionary(f => f.Name, f => f.Text)
                                :
                                null
                      );

    }
}
