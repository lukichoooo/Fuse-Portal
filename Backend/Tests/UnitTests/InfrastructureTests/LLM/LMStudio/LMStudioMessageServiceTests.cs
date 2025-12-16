using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.Cache;
using Core.Interfaces.LLM.LMStudio;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.LLM.LMStudio
{
    [TestFixture]
    public class LMStudioMessageServiceTests
    {
        private readonly LLMApiSettingKeys _settingKeys = new()
        {
            Chat = "chat",
            Parser = "parsar"
        };

        private readonly LLMApiSettings _apiSettings = new()
        {
            URL = "asdadjaod",
            ChatRoute = "/v1/chat/completions",

            Model = "qwen2.5-7b-instruct",
            TimeoutMins = 1,

            Temperature = 0.7f,
            MaxTokens = 2048,
            Stream = false
        };


        private readonly Fixture _fix = new();
        private LMStudioMapper _mapper;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var inputGenMock = new Mock<ILLMInputGenerator>();
            inputGenMock.Setup(g => g.GenerateInput(It.IsAny<MessageDto>(), It.IsAny<string>()))
                .Returns("INPUT");
            var keyOptions = Options.Create(_settingKeys);

            var settingsChooserMock = new Mock<ILLMApiSettingsChooser>();
            settingsChooserMock.Setup(s => s.GetSettings(It.IsAny<string>()))
                .Returns(_apiSettings);


            _mapper = new LMStudioMapper(
                    inputGenMock.Object,
                    settingsChooserMock.Object,
                    keyOptions
                    );

        }

        private ILLMMessageService CreateService(ILMStudioApi api, IChatMetadataService metadataService)
        {
            var settingKeys = _fix.Create<LLMApiSettingKeys>();
            var keyOptionsMock = new Mock<IOptions<LLMApiSettingKeys>>();
            keyOptionsMock.Setup(x => x.Value)
                .Returns(settingKeys);
            return new LMStudioMessageService(api, _mapper, metadataService, keyOptionsMock.Object);
        }

        [Test]
        public async Task SendMessageAsync_Success()
        {
            var msg = _fix.Create<MessageDto>();
            var request = _fix.Create<LMStudioRequest>();
            var response = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()))
                .ReturnsAsync(response);

            var dataServiceMock = new Mock<IChatMetadataService>();
            dataServiceMock.Setup(a => a.GetLastResponseIdAsync(It.IsAny<int>()))
                .ReturnsAsync("id");
            dataServiceMock.Setup(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()));

            var service = CreateService(apiMock.Object, dataServiceMock.Object);

            var res = await service.SendMessageAsync(msg, null);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ChatId, Is.EqualTo(msg.ChatId));
            AssertServiceCalls(apiMock, dataServiceMock);
        }


        [Test]
        public async Task SendMessageAsync_NullLastResponseId_Success()
        {
            var msg = _fix.Create<MessageDto>();
            var request = _fix.Create<LMStudioRequest>();
            var response = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()))
                .ReturnsAsync(response);

            var dataServiceMock = new Mock<IChatMetadataService>();
            dataServiceMock.Setup(a => a.GetLastResponseIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => null);
            dataServiceMock.Setup(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()));

            var service = CreateService(apiMock.Object, dataServiceMock.Object);

            var res = await service.SendMessageAsync(msg, null);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ChatId, Is.EqualTo(msg.ChatId));
            AssertServiceCalls(apiMock, dataServiceMock);
        }



        [Test]
        public async Task SendMessageAsync_WithStreaming_Success()
        {
            var msg = _fix.Create<MessageDto>();
            var request = _fix.Create<LMStudioRequest>();
            var action = _fix.Create<Func<string, Task>>();
            var response = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageWithStreamingAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>(),
                        action))
                .ReturnsAsync(response);

            var dataServiceMock = new Mock<IChatMetadataService>();
            dataServiceMock.Setup(a => a.GetLastResponseIdAsync(It.IsAny<int>()))
                .ReturnsAsync("id");
            dataServiceMock.Setup(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()));

            var service = CreateService(apiMock.Object, dataServiceMock.Object);

            var res = await service.SendMessageAsync(msg, action);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ChatId, Is.EqualTo(msg.ChatId));
            AssertStreamServiceCalls(apiMock, dataServiceMock, action);
        }


        [Test]
        public async Task SendMessageAsync_WithStreaming_NullLastResponseId_Success()
        {
            var msg = _fix.Create<MessageDto>();
            var request = _fix.Create<LMStudioRequest>();
            var action = _fix.Create<Func<string, Task>>();
            var response = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageWithStreamingAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>(),
                        action))
                .ReturnsAsync(response);

            var dataServiceMock = new Mock<IChatMetadataService>();
            dataServiceMock.Setup(a => a.GetLastResponseIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => null);
            dataServiceMock.Setup(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()));

            var service = CreateService(apiMock.Object, dataServiceMock.Object);

            var res = await service.SendMessageAsync(msg, action);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ChatId, Is.EqualTo(msg.ChatId));
            AssertStreamServiceCalls(apiMock, dataServiceMock, action);
        }

        private static void AssertStreamServiceCalls(
                Mock<ILMStudioApi> apiMock,
                Mock<IChatMetadataService> dataServiceMock,
                Func<string, Task>? action = null)
        {
            apiMock.Verify(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()),
                        Times.Never());
            apiMock.Verify(a => a.SendMessageWithStreamingAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>(),
                        action),
                        Times.Once());
            dataServiceMock.Verify(a => a.GetLastResponseIdAsync(It.IsAny<int>()),
                        Times.Once());
            dataServiceMock.Verify(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()),
                        Times.Once());
        }

        private static void AssertServiceCalls(
                Mock<ILMStudioApi> apiMock,
                Mock<IChatMetadataService> dataServiceMock)
        {
            apiMock.Verify(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()),
                        Times.Once());
            apiMock.Verify(a => a.SendMessageWithStreamingAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>(),
                        It.IsAny<Func<string, Task>>()),
                        Times.Never());
            dataServiceMock.Verify(a => a.GetLastResponseIdAsync(It.IsAny<int>()),
                        Times.Once());
            dataServiceMock.Verify(a => a.SetLastResponseIdAsync(
                        It.IsAny<int>(), It.IsAny<string>()),
                        Times.Once());
        }

    }
}
