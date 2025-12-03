using AutoFixture;
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
        private static readonly Fixture _globalFixture = new();

        [SetUp]
        public void BeforeAll()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // random Id
                .Options;
            _context = new MyContext(options);
            _repo = new ChatRepo(_context);
        }

        [TearDown]
        public async Task AfterAllAsync()
        {
            await _context.DisposeAsync();
        }


        private static Chat CreateChatById(int id)
            => _globalFixture.Build<Chat>()
            .With(c => c.Messages, [])
            .With(c => c.Id, id)
            .Create();

        private static ChatFile CreateChatFileById(int id)
            => _globalFixture.Build<ChatFile>()
            .With(cf => cf.Id, id)
            .Create();

        private static Message CreateMessageById(int id)
            => _globalFixture.Build<Message>()
            .With(m => m.Files, [])
            .With(m => m.Chat, CreateChatById(id))
            .With(m => m.Id, id)
            .Create();



        [Test]
        public async Task AddMessageAsync_Success()
        {
            var id = _globalFixture.Create<int>();
            var msg = CreateMessageById(id);

            var rv = await _repo.AddMessageAsync(msg);
            var res = await _context.Messages.FindAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(rv, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
            Assert.That(rv.Id, Is.EqualTo(id));
        }



        [Test]
        public async Task DeleteMessageAsync_Success()
        {
            var id = _globalFixture.Create<int>();
            var msg = CreateMessageById(id);
            await _context.Messages.AddAsync(msg);
            await _context.SaveChangesAsync();

            var rv = await _repo.DeleteMessageByIdAsync(id);
            var res = await _context.Messages.FindAsync(id);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Null);
            Assert.That(rv.Id, Is.EqualTo(id));
        }



        [Test]
        public async Task DeleteMessageAsync_NotFound_Throws()
        {
            var id = _globalFixture.Create<int>();
            var msg = CreateMessageById(id);

            Assert.ThrowsAsync<MessageNotFoundException>(async () =>
                    await _repo.DeleteMessageByIdAsync(id));
        }



        [Test]
        public async Task GetChatAsync_Success()
        {
            var id = _globalFixture.Create<int>();
            var chat = CreateChatById(id);
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            var res = await _repo.GetChatByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetChatAsync_NotFound_Null()
        {
            var id = _globalFixture.Create<int>();
            var res = await _repo.GetChatByIdAsync(id);
            Assert.That(res, Is.Null);
        }


        [TestCase(0)]
        [TestCase(3)]
        public async Task GetAllChatsPageAsync_Success(int repeat)
        {
            const int lastId = int.MinValue, pageSize = 16;
            var ogRepeat = _globalFixture.RepeatCount;
            _globalFixture.RepeatCount = repeat;
            var chats = _globalFixture.Build<Chat>()
                .With(c => c.Messages, [])
                .CreateMany()
                .ToList();
            _globalFixture.RepeatCount = ogRepeat;

            await _context.Chats.AddRangeAsync(chats);
            await _context.SaveChangesAsync();

            var res = await _repo.GetAllChatsPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ConvertAll(c => c.Id)
                    .Order(),
                    Is.EquivalentTo(chats.ConvertAll(c => c.Id)
                        .Order()));
        }

        [TestCase(0)]
        [TestCase(3)]
        public async Task GetMessagesForChat_Success(int repeat)
        {
            const int lastId = int.MinValue, pageSize = 16;
            const int chatId = 5;
            const int otherChatId = 7;
            var chat = CreateChatById(chatId);
            var otherChat = CreateChatById(otherChatId);

            var fixture = new Fixture() { RepeatCount = repeat };
            var messages = fixture.Build<Message>()
                .With(m => m.Chat, chat)
                .With(m => m.ChatId, chat.Id)
                .With(m => m.Files, [])
                .CreateMany()
                .ToList();
            var otherMessages = fixture.Build<Message>()
                .With(m => m.Chat, otherChat)
                .With(m => m.ChatId, otherChat.Id)
                .With(m => m.Files, [])
                .CreateMany()
                .ToList();


            await _context.Chats.AddAsync(chat);
            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            var res = await _repo.GetMessagesForChat(chat.Id, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ConvertAll(m => m.Id)
                .Order(),
                Is.EqualTo(messages.ConvertAll(m => m.Id)
                    .Order()));
        }

    }
}
