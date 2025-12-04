using Moq;
using AutoFixture;
using Infrastructure.Services;
using Core.Interfaces.Convo;
using Core.Entities.Convo;
using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces.LLM;
using Core.Interfaces.Convo.FileServices;

namespace InfrastructureTests.Convo
{
    [TestFixture]
    public class ChatServiceTests
    {
        private static readonly Fixture _globalFixture = new();
        private readonly IChatMapper _mapper = new ChatMapper();

        private IChatService CreateService(
                IChatRepo? repo = null,
                ILLMService? LLMservice = null,
                IFileProcessingService? fileService = null)
        {
            repo ??= new Mock<IChatRepo>().Object;
            LLMservice ??= new Mock<ILLMService>().Object;
            fileService ??= new Mock<IFileProcessingService>().Object;
            return new ChatService(repo, LLMservice, _mapper, fileService);
        }

        private static Chat CreateChatById(int id)
            => _globalFixture.Build<Chat>()
            .With(c => c.Messages, [])
            .With(c => c.Id, id)
            .Create();

        private static Message CreateMessageById(int id)
            => _globalFixture.Build<Message>()
            .With(c => c.Chat, CreateChatById(id))
            .With(c => c.Files, [])
            .With(c => c.ChatId, id)
            .With(c => c.Id, id)
            .Create();

        [TestCase(0)]
        [TestCase(3)]
        public async Task GetAllChatsPageAsync_Success(int repeat)
        {
            var fixture = new Fixture() { RepeatCount = repeat };
            const int lastId = int.MinValue, pageSize = 16;
            var chats = fixture.Build<Chat>()
                .With(c => c.Messages, [])
                .CreateMany()
                .ToList();
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.GetAllChatsPageAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(chats);
            var service = CreateService(mock.Object);

            List<ChatDto> res = await service.GetAllChatsPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ConvertAll(d => d.Id),
                    Is.EquivalentTo(chats.ConvertAll(d => d.Id)));
        }


        [TestCase(0)]
        [TestCase(3)]
        public async Task GetFullChatPageAsync_Success(int repeat)
        {
            var fixture = new Fixture() { RepeatCount = repeat };
            const int lastId = -1, pageSize = 16;
            int chatId = fixture.Create<int>();
            var chat = CreateChatById(chatId);
            var messages = fixture.Build<Message>()
                .With(m => m.Files, [])
                .With(m => m.Chat, chat)
                .CreateMany()
                .ToList();
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.GetChatByIdAsync(chatId))
                .Returns(new ValueTask<Chat?>(Task.FromResult<Chat?>(chat)));
            mock.Setup(r => r.GetMessagesForChat(chatId, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(messages);
            var service = CreateService(mock.Object);

            ChatFullDto res = await service.GetFullChatPageAsync(chatId, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Messages.ConvertAll(d => d.Id),
                    Is.EquivalentTo(messages.ConvertAll(d => d.Id)));
            mock.Verify(r => r.GetChatByIdAsync(chatId), Times.Once());
            mock.Verify(r =>
                    r.GetMessagesForChat(chatId, It.IsAny<int>(), It.IsAny<int>()),
                    Times.Once());
        }


        [Test]
        public async Task GetFullChatPageAsync_NotFound_Throws()
        {
            const int lastId = -1, pageSize = 16;
            int chatId = _globalFixture.Create<int>();
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.GetChatByIdAsync(chatId))
                .Returns(new ValueTask<Chat?>(Task.FromResult<Chat?>(null)));
            var service = CreateService(mock.Object);

            Assert.ThrowsAsync<ChatNotFoundException>(async () =>
                    await service.GetFullChatPageAsync(chatId, lastId, pageSize));
            mock.Verify(r => r.GetChatByIdAsync(chatId), Times.Once());
            mock.Verify(r =>
                    r.GetMessagesForChat(chatId, It.IsAny<int>(), It.IsAny<int>()),
                    Times.Never());
        }

        [Test]
        public async Task DeleteMessageByIdAsync_Success()
        {
            int msgId = _globalFixture.Create<int>();
            var msg = CreateMessageById(msgId);
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.DeleteMessageByIdAsync(msgId))
                .ReturnsAsync(msg);
            var service = CreateService(mock.Object);

            var res = await service.DeleteMessageByIdAsync(msgId);

            Assert.That(res, Is.Not.Null);
            mock.Verify(r => r.DeleteMessageByIdAsync(msgId), Times.Once());
        }


        [Test]
        public async Task DeleteMessageByIdAsync_NotFound_Throws()
        {
            int msgId = _globalFixture.Create<int>();
            var msg = CreateMessageById(msgId);
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.DeleteMessageByIdAsync(msgId))
                .ThrowsAsync(new MessageNotFoundException());
            var service = CreateService(mock.Object);

            Assert.ThrowsAsync<MessageNotFoundException>(async () =>
                    await service.DeleteMessageByIdAsync(msgId));
        }


        [Test]
        public async Task CreateNewChat()
        {
            var chatId = _globalFixture.Create<int>();
            var chat = CreateChatById(chatId);
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.CreateNewChat(chat.Name))
                .ReturnsAsync(chat);
            var LLMServiceMock = new Mock<ILLMService>();
            var service = CreateService(repoMock.Object, LLMServiceMock.Object);

            var res = await service.CreateNewChat(chat.Name);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(chat.Name));
        }

        [Test]
        public async Task SendMessage_Success_NoFiles()
        {
            var cm = _globalFixture.Create<ClientMessage>();

            var response = _globalFixture.Create<MessageDto>();
            var LLMServiceMock = new Mock<ILLMService>();
            LLMServiceMock.Setup(s => s.SendMessageAsync(It.IsAny<MessageDto>()))
                .ReturnsAsync(response);

            int msgId = _globalFixture.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message msg) => msg);

            var service = CreateService(repoMock.Object, LLMServiceMock.Object);
            var messageRequest = new MessageRequest { Message = cm, FileIds = [] };

            var res = await service.SendMessageAsync(messageRequest);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(response));
            LLMServiceMock.Verify(s => s.SendMessageAsync(It.IsAny<MessageDto>()), Times.Once());
            repoMock.Verify(r => r.AddMessageAsync(It.IsAny<Message>()), Times.Exactly(2));
        }



        [Test]
        public async Task SendMessage_Success_WithFiles()
        {
            var cm = _globalFixture.Create<ClientMessage>();
            var fileIds = _globalFixture.CreateMany<int>().ToList();

            var response = _globalFixture.Create<MessageDto>();
            var LLMServiceMock = new Mock<ILLMService>();
            LLMServiceMock.Setup(s => s.SendMessageAsync(It.IsAny<MessageDto>()))
                .ReturnsAsync(response);

            int msgId = _globalFixture.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message msg) => msg);

            var service = CreateService(repoMock.Object, LLMServiceMock.Object);
            var messageRequest = new MessageRequest { Message = cm, FileIds = fileIds };

            var res = await service.SendMessageAsync(messageRequest);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(response));
            LLMServiceMock.Verify(s => s.SendMessageAsync(It.IsAny<MessageDto>()), Times.Once());
            repoMock.Verify(r => r.AddMessageAsync(It.IsAny<Message>()), Times.Exactly(2));
        }


        [Test]
        public async Task UploadFilesAsync_Success()
        {
            // var fileUploads = _globalFixture.Build<FileUpload>()
            //         .OmitAutoProperties()
            //         .With(fu => fu.Name, _globalFixture.Create<string>())
            //         .CreateMany()
            //         .ToList();

            var fileUploads = new List<FileUpload>();

            var files = _globalFixture.Build<ChatFile>()
                .With(f => f.Message, CreateMessageById(1))
                .CreateMany()
                .ToList();

            var fileMock = new Mock<IFileProcessingService>();
            var fileDtos = files.ConvertAll(_mapper.ToFileDto);
            fileMock.Setup(fs => fs.ProcessFilesAsync(It.IsAny<List<FileUpload>>()))
                    .ReturnsAsync(fileDtos);

            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.AddFilesAsync(It.IsAny<List<ChatFile>>()))
                .ReturnsAsync((List<ChatFile> fs) => fs);

            var service = CreateService(
                    repo: repoMock.Object,
                    fileService: fileMock.Object
                    );

            var res = await service.UploadFilesForMessageAsync(fileUploads);

            Assert.That(res, Is.Not.Null);
            // Assert.That(res.Order(),
            //         Is.EqualTo(files.ConvertAll(f => f.Id)
            //             .Order()));
        }



        [Test]
        public async Task RemoveFileAsync_Success()
        {
            var fileReponse = _globalFixture.Build<ChatFile>()
                .With(cf => cf.Message, CreateMessageById(0))
                .Create();
            var fileId = _globalFixture.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.RemoveFileByIdAsync(fileId))
                .ReturnsAsync(fileReponse);
            var service = CreateService(repoMock.Object);

            var res = await service.RemoveFileAsync(fileId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(fileReponse.Name));
        }



        [Test]
        public async Task RemoveFileAsync_NotFound_Throws()
        {
            var fileId = _globalFixture.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.RemoveFileByIdAsync(fileId))
                .ThrowsAsync(new FileNotFoundException());
            var service = CreateService(repoMock.Object);

            Assert.ThrowsAsync<FileNotFoundException>(async () =>
                    await service.RemoveFileAsync(fileId));
        }
    }
}
