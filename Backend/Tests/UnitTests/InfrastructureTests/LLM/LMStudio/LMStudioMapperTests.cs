using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.LLM
{
    [TestFixture]
    public class LMStudioMapperTests
    {
        private readonly LMStudioApiSettings _apiSettings = new()
        {
            URL = "asdadjaod",
            ChatRoute = "/v1/chat/completions",

            Model = "qwen2.5-7b-instruct",

            Temperature = 0.7f,
            MaxTokens = 2048,
            Stream = false
        };

        private readonly LLMInputSettings _inputSettings = new()
        {
            SystemPromptDelimiter = "---RULES---",
            UserInputDelimiter = "---USER INPUT---",
            FileNameDelimiter = "---FILE NAME---",
            FileContentDelimiter = "---FILE CONTENT---",
            SystemPrompt = "you are a cool guy with black glasses"
        };

        private readonly Fixture _globalFixture = new();
        private LMStudioMapper _mapper;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var mock = new Mock<ILLMInputGenerator>();
            mock.Setup(g => g.GenerateInput(It.IsAny<MessageDto>(), It.IsAny<string>()))
                .Returns("INPUT");
            var apiOptions = Options.Create(_apiSettings);
            var inputOptions = Options.Create(_inputSettings);
            _mapper = new LMStudioMapper(apiOptions, inputOptions, mock.Object);
        }

        [Test]
        public void ToMessageDto_From_Response()
        {
            var chatId = _globalFixture.Create<int>();
            var response = _globalFixture.Create<LMStudioResponse>();
            var res = _mapper.ToMessageDto(response, chatId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Text,
                    Is.EqualTo(response.Output[0].Content[0].Text));
            Assert.That(res.ChatId, Is.EqualTo(chatId));
        }



        [TestCase("siudahdiudw")]
        [TestCase(null)]
        public void ToRequest_From_MessageDto(string? prevResponseId)
        {
            var msg = _globalFixture.Create<MessageDto>();
            var res = _mapper.ToRequest(msg, prevResponseId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Model, Is.EqualTo(_apiSettings.Model));
            Assert.That(res.Input, Is.Not.Null);
            Assert.That(res.Input, Is.Not.Empty);
            Assert.That(res.PreviousResponseId, Is.EqualTo(prevResponseId));
        }
    }
}
