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

        public async Task<Message> DeleteMessageByIdAsync(int msgId, int userId)
        {
            var msg = await _context.Messages
                .Include(m => m.Chat)
                .FirstOrDefaultAsync(m => m.Id == msgId
                        && m.Chat.UserId == userId)
                ?? throw new MessageNotFoundException($" No message found with Id={msgId}");
            _context.Remove(msg);

            await _context.SaveChangesAsync();
            return msg;
        }

        public async ValueTask<Chat?> GetChatByIdAsync(int chatId, int userId)
            => await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId
                    && c.UserId == userId);

        public Task<List<Chat>> GetAllChatsForUserPageAsync(
                int? lastChatId,
                int pageSize,
                int userId)
        {
            IQueryable<Chat> chats = _context.Chats
                .Where(c => c.UserId == userId);

            if (lastChatId is not null)
            {
                chats = chats
                    .Where(c => c.Id > lastChatId);
            }

            return chats
                .OrderBy(c => c.Id)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Chat> GetChatWithMessagesPageAsync(
            int chatId,
            int? firstMsgId,
            int pageSize,
            int userId)
        {
            IQueryable<Message> messageQuery = _context.Messages
                .Where(m => m.ChatId == chatId);

            if (firstMsgId is not null)
                messageQuery = messageQuery.Where(m => m.Id < firstMsgId);

            var messages = await messageQuery
                .Include(m => m.Files)
                .OrderByDescending(m => m.Id)
                .Take(pageSize)
                .ToListAsync();

            messages.Reverse();

            var chat = await _context.Chats
                .Where(c => c.Id == chatId && c.UserId == userId)
                .FirstOrDefaultAsync()
                ?? throw new ChatNotFoundException($"Chat not found with Id={chatId}");

            chat.Messages = messages;

            return chat;
        }


        public async Task<Chat> UpdateChatLastResponseIdAsync(
                int chatId,
                string newLastResponseId,
                int userId)
        {
            var chat = await _context.Chats
                .FirstOrDefaultAsync(c => c.Id == chatId
                        && c.UserId == userId)
                ?? throw new ChatNotFoundException($" No Chat found with Id={chatId}, userId={userId}");
            chat.LastResponseId = newLastResponseId;

            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<Chat> CreateNewChatAsync(Chat chat)
        {
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

        public async Task<ChatFile> RemoveFileByIdAsync(int fileId, int userId)
        {
            var file = await _context.ChatFiles
                .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId)
                ?? throw new FileNotFoundException($"File with Id={fileId}, userId={userId} Not Found");
            _context.Remove(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async ValueTask<ChatFile?> GetFileByIdAsync(int fileId, int userId)
            => await _context.ChatFiles
            .FirstOrDefaultAsync(f => f.Id == fileId
                    && f.UserId == userId);

        public async Task<ChatFile> AddStoredFileToMessage(
                int fileId,
                int messageId,
                int userId)
        {
            var file = await _context.ChatFiles
                    .FirstOrDefaultAsync(f => f.Id == fileId
                            && f.UserId == userId)
                ?? throw new FileNotFoundException($"File not found With Id={fileId}");
            if (file.MessageId != messageId) // only update if it's different
            {
                file.MessageId = messageId;
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
            return file;
        }
    }
}
