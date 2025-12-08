using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Exceptions;
using Core.Interfaces.Convo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Presentation.Controllers;

namespace PresentationTests
{
    [TestFixture]
    public class ChatControllerTests
    {
        private readonly Fixture _gloablFixture = new();
        private static readonly ControllerSettings _settings = new()
        {
            DefaultPageSize = 16,
            SmallPageSize = 8,
            BigPageSize = 32
        };
        private static ChatController CreateController(IChatService service)
            => new(service, Options.Create(_settings));

        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(null, 1)]
        [TestCase(1, 1)]
        public async Task GetAllChatsPageAsync_Success(int? lastId, int? pageSize)
        {
            var chats = _gloablFixture.CreateMany<ChatDto>()
                .ToList();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.GetAllChatsPageAsync(lastId, It.IsAny<int>()))
                .ReturnsAsync(chats);
            var sut = CreateController(mock.Object);

            var res = await sut.GetAllChatsPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as List<ChatDto>;
            Assert.That(actual, Is.EqualTo(chats));
        }



        [TestCase(null, null)]
        [TestCase(1, 1)]
        public async Task GetFullChatPageAsync_Success(int? lastId, int? pageSize)
        {
            var chat = _gloablFixture.Create<ChatFullDto>();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.GetChatWithMessagesPageAsync(chat.Id, lastId, It.IsAny<int>()))
                .ReturnsAsync(chat);
            var sut = CreateController(mock.Object);

            var res = await sut.GetFullChatPageAsync(chat.Id, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as ChatFullDto;
            Assert.That(actual, Is.EqualTo(chat));
        }


        [TestCase(null, null)]
        [TestCase(1, 1)]
        public async Task GetFullChatPageAsync_NotFound_Throws(int? lastId, int? pageSize)
        {
            var chat = _gloablFixture.Create<ChatFullDto>();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.GetChatWithMessagesPageAsync(chat.Id, lastId, It.IsAny<int>()))
                .ThrowsAsync(new ChatNotFoundException());
            var sut = CreateController(mock.Object);

            Assert.ThrowsAsync<ChatNotFoundException>(async () =>
                    await sut.GetFullChatPageAsync(chat.Id, lastId, pageSize));
        }


        [Test]
        public async Task CreateNewChat_Success()
        {
            var chat = _gloablFixture.Create<ChatDto>();
            var request = _gloablFixture.Create<CreateChatRequest>();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.CreateNewChatAsync(request))
                .ReturnsAsync(chat);
            var sut = CreateController(mock.Object);

            var res = await sut.CreateNewChatAsync(request);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(chat));
        }


        [TestCase(1)]
        public async Task DeleteMessageByIdAsync_Success(int id)
        {
            var message = _gloablFixture.Create<MessageDto>();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.DeleteMessageByIdAsync(id))
                .ReturnsAsync(message);
            var sut = CreateController(mock.Object);

            var res = await sut.DeleteMessageByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            var actual = (res.Result as OkObjectResult)?.Value as MessageDto;
            Assert.That(actual, Is.EqualTo(message));
        }


        [TestCase(1)]
        public async Task DeleteMessageByIdAsync_NotFound_Throws(int msgId)
        {
            var message = _gloablFixture.Create<MessageDto>();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.DeleteMessageByIdAsync(msgId))
                .ThrowsAsync(new MessageNotFoundException());
            var sut = CreateController(mock.Object);

            Assert.ThrowsAsync<MessageNotFoundException>(async () =>
                    await sut.DeleteMessageByIdAsync(msgId));
        }



        [Test]
        public async Task SendMessageAsync_Success()
        {
            var request = _gloablFixture.Create<MessageRequest>();
            var message = _gloablFixture.Create<MessageDto>();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.SendMessageAsync(request))
                .ReturnsAsync(message);
            var sut = CreateController(mock.Object);

            var res = await sut.SendMessageAsync(request);

            Assert.That(res, Is.Not.Null);
            var okResult = res.Result as OkObjectResult;
            var actual = okResult?.Value as MessageDto;
            Assert.That(actual, Is.EqualTo(message));
        }



        [Test]
        public async Task SendMessageAsync_ChatNotFound_Throws()
        {
            var request = _gloablFixture.Create<MessageRequest>();
            var mock = new Mock<IChatService>();
            mock.Setup(s => s.SendMessageAsync(request))
                .ThrowsAsync(new ChatNotFoundException());
            var sut = CreateController(mock.Object);

            Assert.ThrowsAsync<ChatNotFoundException>(async () =>
                    await sut.SendMessageAsync(request));
        }

        [Test]
        public async Task UploadFilesAsync_Succes()
        {
            var file1Content = new byte[] { 1, 2, 3 };
            var file2Content = new byte[] { 4, 5, 6 };

            var file1 = new FormFile(new MemoryStream(file1Content), 0, file1Content.Length, "file1", "file1.txt");
            var file2 = new FormFile(new MemoryStream(file2Content), 0, file2Content.Length, "file2", "file2.txt");

            var files = new FormFileCollection { file1, file2 };

            var mockService = new Mock<IChatService>();
            mockService
                .Setup(s => s.UploadFilesAsync(It.IsAny<List<FileUpload>>()))
                .ReturnsAsync([1, 2]);

            var controller = CreateController(mockService.Object);
            var result = await controller.UploadFilesAsync(files);

            // Assert
            var res = result.Result as OkObjectResult;
            Assert.That(res, Is.Not.Null);

            var ids = res.Value as List<int>;
            Assert.That(ids?.Count, Is.EqualTo(files.Count));
        }
    }
}
