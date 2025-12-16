using Moq;
using AutoFixture;
using Infrastructure.Services;
using Core.Interfaces.Convo;
using Core.Entities.Convo;
using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces.LLM;
using Core.Interfaces.Convo.FileServices;
using Core.Interfaces.Auth;

namespace InfrastructureTests.Convo
{
    [TestFixture]
    public class ChatServiceTests
    {
        private static readonly Fixture _fix = new();
        private const int DEFAULT_CONTEXT_ID = 2142321;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        private readonly IChatMapper _mapper = new ChatMapper();

        private IChatService CreateService(
                IChatRepo? repo = null,
                ILLMMessageService? LLMservice = null,
                IFileProcessingService? fileService = null,
                ICurrentContext? currContext = null)
        {
            repo ??= new Mock<IChatRepo>().Object;
            LLMservice ??= new Mock<ILLMMessageService>().Object;
            fileService ??= new Mock<IFileProcessingService>().Object;
            if (currContext is null)
            {
                var mock = new Mock<ICurrentContext>();
                mock.Setup(c => c.GetCurrentUserId())
                    .Returns(DEFAULT_CONTEXT_ID);
                currContext = mock.Object;
            }

            return new ChatService(repo, LLMservice, _mapper, fileService, currContext);
        }


        [TestCase(0)]
        [TestCase(3)]
        public async Task GetAllChatsPageAsync_Success(int repeat)
        {
            var fixture = new Fixture() { RepeatCount = repeat };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            const int lastId = int.MinValue, pageSize = 16;
            var chats = fixture.Build<Chat>()
                .With(c => c.Messages, [])
                .CreateMany()
                .ToList();
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.GetAllChatsForUserPageAsync(It.IsAny<int>(), It.IsAny<int>(),
                        DEFAULT_CONTEXT_ID))
                .ReturnsAsync(chats);
            var service = CreateService(mock.Object);

            List<ChatDto> res = await service.GetAllChatsPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ConvertAll(d => d.Id),
                    Is.EquivalentTo(chats.ConvertAll(d => d.Id)));
        }


        [TestCase(0)]
        [TestCase(3)]
        public async Task GetChatWithMessagesPageAsync_Success(int repeat)
        {
            var fixture = new Fixture() { RepeatCount = repeat };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            const int lastId = -1, pageSize = 16;
            int chatId = fixture.Create<int>();
            var chat = _fix.Build<Chat>()
                .With(c => c.UserId, DEFAULT_CONTEXT_ID)
                .With(c => c.Id, chatId)
                .Create();
            var messages = fixture.Build<Message>()
                .With(m => m.Chat, chat)
                .With(m => m.ChatId, chatId)
                .CreateMany()
                .ToList();
            chat.Messages = messages;
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.GetChatWithMessagesPageAsync(chatId, It.IsAny<int>(),
                        It.IsAny<int>(), DEFAULT_CONTEXT_ID))
                .ReturnsAsync(chat);
            var service = CreateService(mock.Object);

            ChatFullDto res = await service.GetChatWithMessagesPageAsync(chatId, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Messages.ConvertAll(d => d.Id),
                    Is.EquivalentTo(messages.ConvertAll(d => d.Id)));
        }

        [Test]
        public async Task DeleteMessageByIdAsync_Success()
        {
            int msgId = _fix.Create<int>();
            var msg = _fix.Build<Message>()
                .With(m => m.Id, msgId)
                .Create();
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.DeleteMessageByIdAsync(msgId, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(msg);
            var service = CreateService(mock.Object);

            var res = await service.DeleteMessageByIdAsync(msgId);

            Assert.That(res, Is.Not.Null);
            mock.Verify(r => r.DeleteMessageByIdAsync(
                        msgId,
                        DEFAULT_CONTEXT_ID),
                    Times.Once());
        }


        [Test]
        public async Task DeleteMessageByIdAsync_NotFound_Throws()
        {
            int msgId = _fix.Create<int>();
            var msg = _fix.Build<Message>()
                .With(m => m.Id, msgId)
                .Create();
            var mock = new Mock<IChatRepo>();
            mock.Setup(r => r.DeleteMessageByIdAsync(msgId, DEFAULT_CONTEXT_ID))
                .ThrowsAsync(new MessageNotFoundException());
            var service = CreateService(mock.Object);

            Assert.ThrowsAsync<MessageNotFoundException>(async () =>
                    await service.DeleteMessageByIdAsync(msgId));
        }


        [Test]
        public async Task CreateNewChat()
        {
            var chatId = _fix.Create<int>();
            var request = _fix.Create<CreateChatRequest>();
            var chat = _fix.Build<Chat>()
                .With(c => c.UserId, DEFAULT_CONTEXT_ID)
                .With(c => c.Id, chatId)
                .Create();
            chat.Name = request.ChatName;

            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.CreateNewChatAsync(It.IsAny<Chat>()))
                .ReturnsAsync(chat);

            var LLMServiceMock = new Mock<ILLMMessageService>();
            var service = CreateService(repoMock.Object, LLMServiceMock.Object);

            var res = await service.CreateNewChatAsync(request);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(chat.Name));
        }

        [Test]
        public async Task SendMessage_Success_NoFiles()
        {
            var chatid = _fix.Create<int>();
            var text = _fix.Create<string>();
            ClientMessage clientMessage = new()
            {
                Text = text,
                ChatId = chatid
            };
            var fileIds = _fix.CreateMany<int>().ToList();
            MessageRequest messageRequest = new()
            {
                FileIds = fileIds,
                Message = clientMessage,
            };

            var llmResponse = _fix.Build<MessageDto>()
                .With(m => m.FromUser, false)
                .Create();
            var LLMServiceMock = new Mock<ILLMMessageService>();
            LLMServiceMock.Setup(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        It.IsAny<Func<string, Task>>()))
                .ReturnsAsync(llmResponse);

            int msgId = _fix.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message msg) => msg);

            var service = CreateService(repoMock.Object, LLMServiceMock.Object);

            var res = await service.SendMessageAsync(messageRequest, null);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Response.Text, Is.EqualTo(llmResponse.Text));
            Assert.That(res.Response.FromUser, Is.EqualTo(false));
            Assert.That(res.UserMessage.Text, Is.EqualTo(clientMessage.Text));
            Assert.That(res.UserMessage.FromUser, Is.EqualTo(true));
            LLMServiceMock.Verify(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        It.IsAny<Func<string, Task>>()),
                    Times.Once());
            repoMock.Verify(r => r.AddMessageAsync(It.IsAny<Message>()), Times.Exactly(2));
        }



        [Test]
        public async Task SendMessage_Success_WithFiles()
        {
            var cm = _fix.Create<ClientMessage>();
            var fileIds = _fix.CreateMany<int>().ToList();

            var response = _fix.Create<MessageDto>();
            var LLMServiceMock = new Mock<ILLMMessageService>();
            LLMServiceMock.Setup(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        It.IsAny<Func<string, Task>>()))
                .ReturnsAsync(response);

            int msgId = _fix.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message msg) => msg);

            var service = CreateService(repoMock.Object, LLMServiceMock.Object);
            var messageRequest = new MessageRequest { Message = cm, FileIds = fileIds };

            var res = await service.SendMessageAsync(messageRequest, null);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Response.Text, Is.EqualTo(response.Text));
            Assert.That(res.UserMessage.Text, Is.EqualTo(cm.Text));
            LLMServiceMock.Verify(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        null),
                    Times.Once());
            LLMServiceMock.Verify(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        It.Is<Func<string, Task>>(a => a != null)),
                    Times.Never());
            repoMock.Verify(r => r.AddMessageAsync(It.IsAny<Message>()), Times.Exactly(2));
        }


        [Test]
        public async Task SendMessageAsync_Success_Streaming()
        {
            var chatid = _fix.Create<int>();
            var text = _fix.Create<string>();
            ClientMessage clientMessage = new()
            {
                Text = text,
                ChatId = chatid
            };
            var fileIds = _fix.CreateMany<int>().ToList();
            MessageRequest messageRequest = new()
            {
                FileIds = fileIds,
                Message = clientMessage,
            };
            var action = _fix.Create<Func<string, Task>>();

            var responseMessageDto = _fix.Build<MessageDto>()
                .With(m => m.FromUser, false)
                .Create();
            var LLMServiceMock = new Mock<ILLMMessageService>();
            LLMServiceMock.Setup(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        action))
                .ReturnsAsync(responseMessageDto);

            int msgId = _fix.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message msg) => msg);

            var service = CreateService(repoMock.Object, LLMServiceMock.Object);

            var res = await service.SendMessageAsync(messageRequest, action);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Response.Text, Is.EqualTo(responseMessageDto.Text));
            Assert.That(res.Response.FromUser, Is.EqualTo(false));
            Assert.That(res.UserMessage.Text, Is.EqualTo(clientMessage.Text));
            Assert.That(res.UserMessage.FromUser, Is.EqualTo(true));
            LLMServiceMock.Verify(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        null),
                    Times.Never());
            LLMServiceMock.Verify(s => s.SendMessageAsync(
                        It.IsAny<MessageDto>(),
                        action),
                    Times.Once());
            repoMock.Verify(r => r.AddMessageAsync(It.IsAny<Message>()),
                    Times.Exactly(2));
        }






        [Test]
        public async Task UploadFilesAsync_Success()
        {
            var fileUploads = new List<FileUpload>();
            var files = _fix.CreateMany<ChatFile>().ToList();
            foreach (var f in files)
            {
                f.UserId = DEFAULT_CONTEXT_ID;
            }

            var fileMock = new Mock<IFileProcessingService>();
            var fileDtos = files.ConvertAll(_mapper.ToFileDto);
            fileMock.Setup(fs => fs.ProcessFilesAsync(It.IsAny<List<FileUpload>>()))
                    .ReturnsAsync(fileDtos);

            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.AddFilesAsync(It.IsAny<List<ChatFile>>()))
                .ReturnsAsync(files);

            var service = CreateService(
                    repo: repoMock.Object,
                    fileService: fileMock.Object
                    );

            var res = await service.UploadFilesAsync(fileUploads);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Order(),
                    Is.EqualTo(files.ConvertAll(f => f.Id)
                        .Order()));
        }



        [Test]
        public async Task RemoveFileAsync_Success()
        {
            var fileReponse = _fix.Create<ChatFile>();
            fileReponse.UserId = DEFAULT_CONTEXT_ID;
            var fileId = _fix.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.RemoveFileByIdAsync(fileId, DEFAULT_CONTEXT_ID))
                .ReturnsAsync(fileReponse);
            var service = CreateService(repoMock.Object);

            var res = await service.RemoveFileAsync(fileId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(fileReponse.Name));
        }



        [Test]
        public async Task RemoveFileAsync_NotFound_Throws()
        {
            var fileId = _fix.Create<int>();
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.RemoveFileByIdAsync(fileId, DEFAULT_CONTEXT_ID))
                .ThrowsAsync(new FileNotFoundException());
            var service = CreateService(repoMock.Object);

            Assert.ThrowsAsync<FileNotFoundException>(async () =>
                    await service.RemoveFileAsync(fileId));
        }
    }
}
