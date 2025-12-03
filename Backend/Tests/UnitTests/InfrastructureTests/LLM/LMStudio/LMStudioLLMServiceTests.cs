using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.Cache;
using Core.Interfaces.LLM.LMStudio;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.LLM.LMStudio
{
    [TestFixture]
    public class LMStudioLLMServiceTests
    {
        private readonly LMStudioApiSettings _settings = new()
        {
            URL = "asdadjaod",
            ChatRoute = "/v1/chat/completions",

            Model = "qwen2.5-7b-instruct",
            SystemPrompt = "never talk about LMStudioMapper",

            Temperature = 0.7f,
            MaxTokens = 2048,
            Stream = false
        };

        private readonly Fixture _globalFixture = new();
        private LMStudioMapper _mapper;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var mock = new Mock<ILLMInputGenerator>();
            mock.Setup(g => g.GenerateInput(It.IsAny<MessageDto>(), It.IsAny<string>()))
                .Returns("INPUT");
            var options = Options.Create(_settings);
            _mapper = new LMStudioMapper(options, mock.Object);
        }

        private ILLMService CreateService(ILMStudioApi api, IChatMetadataService metadataService)
            => new LMStudioLLMService(api, _mapper, metadataService);

        [Test]
        public async Task SendMessageAsync_Success()
        {
            var msg = _globalFixture.Create<MessageDto>();
            var request = _globalFixture.Create<LMStudioRequest>();
            var response = _globalFixture.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>()))
                .ReturnsAsync(response);

            var dataServiceMock = new Mock<IChatMetadataService>();
            dataServiceMock.Setup(a => a.GetLastResponseIdAsync(It.IsAny<int>()))
                .ReturnsAsync("id");
            dataServiceMock.Setup(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()));

            var service = CreateService(apiMock.Object, dataServiceMock.Object);

            var res = await service.SendMessageAsync(msg);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ChatId, Is.EqualTo(msg.ChatId));
            apiMock.Verify(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>()),
                        Times.Once());
            dataServiceMock.Verify(a => a.GetLastResponseIdAsync(It.IsAny<int>()),
                        Times.Once());
            dataServiceMock.Verify(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()),
                        Times.Once());
        }


        [Test]
        public async Task SendMessageAsync_NullLastResponseId_Success()
        {
            var msg = _globalFixture.Create<MessageDto>();
            var request = _globalFixture.Create<LMStudioRequest>();
            var response = _globalFixture.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>()))
                .ReturnsAsync(response);

            var dataServiceMock = new Mock<IChatMetadataService>();
            dataServiceMock.Setup(a => a.GetLastResponseIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => null);
            dataServiceMock.Setup(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()));

            var service = CreateService(apiMock.Object, dataServiceMock.Object);

            var res = await service.SendMessageAsync(msg);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ChatId, Is.EqualTo(msg.ChatId));
            apiMock.Verify(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>()),
                        Times.Once());
            dataServiceMock.Verify(a => a.GetLastResponseIdAsync(It.IsAny<int>()),
                        Times.Once());
            dataServiceMock.Verify(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()),
                        Times.Once());
        }
    }
}
