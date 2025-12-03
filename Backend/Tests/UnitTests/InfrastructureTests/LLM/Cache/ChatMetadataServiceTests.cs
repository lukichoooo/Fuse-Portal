using AutoFixture;
using Core.Entities.Convo;
using Core.Interfaces.Convo;
using Core.Interfaces.LLM.Cache;
using Infrastructure.Services.LLM.Cache;
using Moq;

namespace InfrastructureTests.LLM.Cache
{
    [TestFixture]
    public class ChatMetadataServiceTests
    {
        private readonly Fixture _globalFixture = new();

        private static IChatMetadataService CreateService(IChatRepo repo, IChatMetadataCache cache)
            => new ChatMetadataService(repo, cache);



        [Test]
        public async Task GetLastResponseIdAsync_Success()
        {
            int chatId = _globalFixture.Create<int>();
            var lastResponseId = _globalFixture.Create<string>();

            var cacheMock = new Mock<IChatMetadataCache>();
            cacheMock.Setup(c => c.GetValueAsync(chatId))
                    .ReturnsAsync(lastResponseId);
            var repoMock = new Mock<IChatRepo>();
            var service = CreateService(repoMock.Object, cacheMock.Object);

            var res = await service.GetLastResponseIdAsync(chatId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(lastResponseId));
            repoMock.Verify(r => r.GetChatByIdAsync(chatId), Times.Never());
            cacheMock.Verify(r => r.GetValueAsync(chatId), Times.Once());
        }

        [Test]
        public async Task GetLastResponseIdAsync_NotCached_Success()
        {
            var chat = _globalFixture.Build<Chat>()
                .With(c => c.Messages, [])
                .Create();
            int chatId = chat.Id;

            var cacheMock = new Mock<IChatMetadataCache>();
            cacheMock.Setup(c => c.GetValueAsync(chatId))
                    .ReturnsAsync(() => null);
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.GetChatByIdAsync(chatId))
                .ReturnsAsync(chat);
            var service = CreateService(repoMock.Object, cacheMock.Object);

            var res = await service.GetLastResponseIdAsync(chatId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(chat.LastResponseId));
            repoMock.Verify(r => r.GetChatByIdAsync(chatId), Times.Once());
            cacheMock.Verify(r => r.GetValueAsync(chatId), Times.Once());
        }


        [Test]
        public async Task GetLastResponseIdAsync_NotAnywhere_Null()
        {
            int chatId = _globalFixture.Create<int>();

            var cacheMock = new Mock<IChatMetadataCache>();
            var repoMock = new Mock<IChatRepo>();
            var service = CreateService(repoMock.Object, cacheMock.Object);

            var res = await service.GetLastResponseIdAsync(chatId);

            Assert.That(res, Is.Null);
            repoMock.Verify(r => r.GetChatByIdAsync(chatId), Times.Once());
            cacheMock.Verify(r => r.GetValueAsync(chatId), Times.Once());
        }



        [Test]
        public async Task SetLastResponseIdAsync_Success()
        {
            int chatId = _globalFixture.Create<int>();
            var chat = _globalFixture.Build<Chat>()
                .With(c => c.Messages, [])
                .Create();
            var lastResponseId = _globalFixture.Create<string>();

            var cacheMock = new Mock<IChatMetadataCache>();
            cacheMock.Setup(c => c.SetValueAsync(chatId, lastResponseId))
                .Returns(Task.CompletedTask);
            var repoMock = new Mock<IChatRepo>();
            repoMock.Setup(r => r.UpdateChatLastResponseIdAsync(chatId, lastResponseId))
                .ReturnsAsync(chat);
            var service = CreateService(repoMock.Object, cacheMock.Object);

            await service.SetLastResponseIdAsync(chatId, lastResponseId);

            repoMock.Verify(r => r.UpdateChatLastResponseIdAsync(chatId, lastResponseId),
                    Times.Once());
            cacheMock.Verify(c => c.SetValueAsync(chatId, lastResponseId),
                    Times.Once());
        }
    }
}
