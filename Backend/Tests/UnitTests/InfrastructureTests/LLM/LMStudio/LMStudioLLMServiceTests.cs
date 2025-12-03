using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.LMStudio;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.LLM.LMStudio
{
    [TestFixture]
    public class LMStudioLLMServiceTests
    {
        private readonly LMStudioSettings _settings = new()
        {
            URL = "asdadjaod",
            ChatRoute = "/v1/chat/completions",

            Model = "qwen2.5-7b-instruct",
            Rules = "never talk about LMStudioMapper",

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

        private ILLMService CreateService(ILMStudioApi api)
            => new LMStudioLLMService(api, _mapper);

        [Test]
        public async Task SendMessageAsync_Success()
        {
            var msg = _globalFixture.Create<MessageDto>();
            var request = _globalFixture.Create<LMStudioRequest>();
            var response = _globalFixture.Create<LMStudioResponse>();
            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>()))
                .ReturnsAsync(response);
            var service = CreateService(apiMock.Object);

            var res = await service.SendMessageAsync(msg);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ChatId, Is.EqualTo(msg.ChatId));
            apiMock.Verify(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>()),
                        Times.Once());
        }
    }
}
