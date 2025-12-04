using Core.Entities.Convo;
using Core.Exceptions;
using Core.Interfaces.Convo;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class ChatRepo(MyContext context) : IChatRepo
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

        public ValueTask<Chat?> GetChatByIdAsync(int chatId)
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

        public async Task<Chat> UpdateChatLastResponseIdAsync(int chatId, string newLastResponseId)
        {
            var chat = await _context.Chats.FindAsync(chatId)
                ?? throw new ChatNotFoundException($" No Chat found with Id={chatId}");
            chat.LastResponseId = newLastResponseId;

            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<Chat> CreateNewChat(string chatName)
        {
            var chat = new Chat { Name = chatName };
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<List<ChatFile>> AddFilesAsync(List<ChatFile> files)
        {
            await _context.ChatFiles.AddRangeAsync(files);
            await _context.SaveChangesAsync();
            return files;
        }

        public async Task<ChatFile> RemoveFileByIdAsync(int fileId)
        {
            var file = await _context.ChatFiles.FindAsync(fileId)
                ?? throw new FileNotFoundException($"File with Id={fileId} Not Found");
            _context.Remove(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public ValueTask<ChatFile?> GetFileByIdAsync(int fileId)
            => _context.ChatFiles.FindAsync(fileId);

    }
}
