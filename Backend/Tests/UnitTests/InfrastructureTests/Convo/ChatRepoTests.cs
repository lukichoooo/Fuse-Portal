using AutoFixture;
using Core.Dtos;
using Core.Entities.Convo;
using Core.Exceptions;
using Core.Interfaces.Convo;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureTests.Convo
{
    [TestFixture]
    public class ChatRepoTests
    {
        private IChatRepo _repo;
        private MyContext _context;
        private static readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public void BeforeEach()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new MyContext(options);
            _repo = new ChatRepo(_context);
        }

        [TearDown]
        public async Task AfterAllAsync()
        {
            await _context.DisposeAsync();
        }



        [Test]
        public async Task AddMessageAsync_Success()
        {
            var msgId = _fix.Create<int>();
            var msg = _fix.Create<Message>();
            msg.Id = msgId;

            var rv = await _repo.AddMessageAsync(msg);
            var res = await _context.Messages.FindAsync(msgId);

            Assert.That(res, Is.Not.Null);
            Assert.That(rv, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(msgId));
            Assert.That(rv.Id, Is.EqualTo(msgId));
        }



        [Test]
        public async Task DeleteMessageAsync_Success()
        {
            var msgId = _fix.Create<int>();
            var msg = _fix.Create<Message>();
            msg.Id = msgId;
            await _context.Messages.AddAsync(msg);
            await _context.SaveChangesAsync();

            var rv = await _repo.DeleteMessageByIdAsync(msgId, msg.Chat.UserId);
            var res = await _context.Messages.FindAsync(msgId);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Null);
            Assert.That(rv.Id, Is.EqualTo(msgId));
        }



        [Test]
        public async Task DeleteMessageAsync_NotFound_Throws()
        {
            var msgId = _fix.Create<int>();
            var msg = _fix.Create<Message>();
            msg.Id = msgId;

            Assert.ThrowsAsync<MessageNotFoundException>(async () =>
                    await _repo.DeleteMessageByIdAsync(msgId, msg.Chat.UserId));
        }



        [Test]
        public async Task GetChatAsync_Success()
        {
            var chatId = _fix.Create<int>();
            var chat = _fix.Create<Chat>();
            chat.Id = chatId;
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            var res = await _repo.GetChatByIdAsync(chatId, chat.UserId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(chatId));
        }


        [Test]
        public async Task GetChatAsync_NotFound_Null()
        {
            var id = _fix.Create<int>();
            var res = await _repo.GetChatByIdAsync(id, 10);
            Assert.That(res, Is.Null);
        }


        [TestCase(0)]
        [TestCase(3)]
        public async Task GetAllChatsForUserPageAsync_Success(int repeat)
        {
            const int userId = 10;
            const int pageSize = 16;
            int? lastChatId = null;
            var fixture = new Fixture() { RepeatCount = repeat };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            List<Chat> chats = fixture.CreateMany<Chat>().ToList();
            foreach (var c in chats)
            {
                c.UserId = userId;
                c.User = null;
            }

            await _context.Chats.AddRangeAsync(chats);
            await _context.SaveChangesAsync();

            List<Chat> res = await _repo.GetAllChatsForUserPageAsync(lastChatId, pageSize, userId);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                res.ConvertAll(c => c.Name).Order().ToList(),
                Is.EqualTo(chats.ConvertAll(c => c.Name).Order().ToList())
            );
        }

        [TestCase(0)]
        [TestCase(3)]
        public async Task GetChatWithMessagesPageAsync_Success(int repeat)
        {
            const int pageSize = 100;
            int? lastId = null;
            const int chatId = 5;
            const int otherChatId = 7;
            const int userId = 10;


            var fixture = new Fixture() { RepeatCount = repeat };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var chat = fixture.Create<Chat>();
            chat.Id = chatId;
            chat.Messages = [];
            chat.UserId = userId;
            chat.User = null;

            var otherChat = fixture.Create<Chat>();
            otherChat.Id = otherChatId;

            var messages = fixture.CreateMany<Message>().ToList();
            var otherMessages = fixture.CreateMany<Message>().ToList();
            foreach (var m in messages)
            {
                m.ChatId = chat.Id;
                m.Chat = chat;
            }
            foreach (var m in otherMessages)
                m.ChatId = otherChat.Id;

            await _context.Chats.AddAsync(chat);
            await _context.Chats.AddAsync(otherChat);
            await _context.SaveChangesAsync();


            await _context.Messages.AddRangeAsync(messages);
            await _context.Messages.AddRangeAsync(otherMessages);
            await _context.SaveChangesAsync();

            var res = await _repo.GetChatWithMessagesPageAsync(
                    chat.Id, lastId, pageSize, userId);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                res.Messages.ConvertAll(m => m.Id).Order().ToList(),
                Is.EqualTo(messages.ConvertAll(m => m.Id).Order().ToList())
            );
        }


        [TestCase(0)]
        [TestCase(22)]
        [TestCase(16)]
        public async Task GetChatWithMessagesPageAsync_PagingTest(int n)
        {
            const int pageSize = 16;
            int? lastId = null;
            const int chatId = 5;
            const int userId = 10;


            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var chat = fixture.Create<Chat>();
            chat.Id = chatId;
            chat.Messages = [];
            chat.UserId = userId;
            chat.User = null;


            var messages = Enumerable.Range(0, n)
                .ToList()
                .ConvertAll(_ =>
                {
                    var msg = fixture.Create<Message>();
                    msg.ChatId = chat.Id;
                    msg.Chat = chat;
                    return msg;
                });

            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();


            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            HashSet<int> seenId = [];
            for (int i = 0; i < n; i += pageSize)
            {
                var res = await _repo.GetChatWithMessagesPageAsync(
                    chat.Id, lastId, pageSize, userId);
                Assert.That(res, Is.Not.Null);
                foreach (var msg in res.Messages)
                {
                    Assert.That(seenId.Contains(msg.Id), Is.EqualTo(false));
                    seenId.Add(msg.Id);
                }
                lastId = res.Messages.Last().Id;
            }
            Assert.That(seenId, Has.Count.EqualTo(n));
        }


        [TestCase(0)]
        [TestCase(22)]
        [TestCase(16)]
        public async Task GetAllChatsForUserPage_PagingTest(int n)
        {
            const int pageSize = 16;
            int? lastId = null;
            const int userId = 10;


            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());



            var chats = Enumerable.Range(0, n)
                .ToList()
                .ConvertAll(_ =>
                {
                    var chat = fixture.Create<Chat>();
                    chat.Messages = [];
                    chat.UserId = userId;
                    chat.User = null;
                    return chat;
                });

            await _context.Chats.AddRangeAsync(chats);
            await _context.SaveChangesAsync();

            HashSet<int> seenId = [];
            for (int i = 0; i < n; i += pageSize)
            {
                var res = await _repo.GetAllChatsForUserPageAsync(
                    lastId, pageSize, userId);
                Assert.That(res, Is.Not.Null);
                foreach (var chat in res)
                {
                    Assert.That(seenId.Contains(chat.Id), Is.EqualTo(false));
                    seenId.Add(chat.Id);
                }
                lastId = res.Last().Id;
            }
            Assert.That(seenId, Has.Count.EqualTo(n));
        }



        [Test]
        public async Task UpdateChatAsync_Success()
        {
            const int chatId = 5;
            var oldVal = _fix.Create<string>();
            var newVal = _fix.Create<string>();
            var chat = _fix.Build<Chat>()
                .With(c => c.Messages, [])
                .With(c => c.Id, chatId)
                .With(c => c.LastResponseId, oldVal)
                .Create();
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            var res = await _repo.UpdateChatLastResponseIdAsync(chatId, newVal, chat.UserId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.LastResponseId, Is.EqualTo(newVal));
            Assert.That(res.LastResponseId, Is.Not.EqualTo(oldVal));
        }


        [Test]
        public async Task UpdateChatAsync_NotFound_Throws()
        {
            const int chatId = 5;
            var newVal = _fix.Create<string>();
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<ChatNotFoundException>(async () =>
                    await _repo.UpdateChatLastResponseIdAsync(chatId, newVal, 10));
        }

        [Test]
        public async Task CreateNewChat_Success()
        {
            var chat = _fix.Create<Chat>();
            var returnValue = await _repo.CreateNewChatAsync(chat);
            var res = await _context.Chats.FindAsync(returnValue.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(res!.Id));
        }

        [Test]
        public async Task RemoveFileByIdAsync_Success()
        {
            var fileId = _fix.Create<int>();
            var file = _fix.Create<ChatFile>();
            file.Id = fileId;
            await _context.ChatFiles.AddAsync(file);
            await _context.SaveChangesAsync();

            var returnValue = await _repo.RemoveFileByIdAsync(fileId, file.UserId);
            var res = await _context.ChatFiles.FindAsync(fileId);

            Assert.That(res, Is.Null);
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue, Is.EqualTo(file));
        }

        [Test]
        public async Task RemoveFileByIdAsync_NotFound_Throws()
        {
            var fileId = _fix.Create<int>();
            Assert.ThrowsAsync<FileNotFoundException>(async () =>
                    await _repo.RemoveFileByIdAsync(fileId, 10));
        }

        [Test]
        public async Task AddFilesAsync_Success()
        {
            var files = _fix.CreateMany<ChatFile>().ToList();

            var returnValue = await _repo.AddFilesAsync(files);

            Assert.That(returnValue, Is.Not.Null);
            foreach (var f in files)
            {
                var res = await _context.ChatFiles.FindAsync(f.Id);
                Assert.That(res, Is.Not.Null);
            }
        }


    }
}
