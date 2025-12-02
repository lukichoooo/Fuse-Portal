using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class CommunicationRepo(MyContext context) : ICommunicationRepo
    {
        private readonly MyContext _context = context;

        public async Task<Message> AddMessageAsync(Message msg)
        {
            await _context.Messages.AddAsync(msg);
            await _context.SaveChangesAsync();
            return msg;
        }

        public async Task<Message> DeleteMessageByIdAsync(int msgId)
        {
            var msg = await _context.Messages.FindAsync(msgId)
                ?? throw new MessageNotFoundException($" No message found with Id={msgId}");
            _context.Remove(msg);
            await _context.SaveChangesAsync();
            return msg;
        }

        public ValueTask<Chat?> GetChatAsync(int chatId)
            => _context.Chats.FindAsync(chatId);

        public Task<List<Chat>> GetAllChatsPageAsync(int lastId, int pageSize)
            => _context.Chats
            .Where(c => c.Id > lastId)
            .OrderBy(c => c.Id)
            .Take(pageSize)
            .ToListAsync();


        public Task<List<Message>> GetMessagesForChat(int chatId, int lastId, int pageSize)
            => _context.Messages
                    .Where(m => m.ChatId == chatId && m.Id > lastId)
                    .OrderBy(m => m.Id)
                    .Take(pageSize)
                    .ToListAsync();

    }
}
