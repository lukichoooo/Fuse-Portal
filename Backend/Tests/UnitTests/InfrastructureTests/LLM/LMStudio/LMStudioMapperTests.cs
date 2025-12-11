using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.LLM
{
    [TestFixture]
    public class LMStudioMapperTests
    {

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

        private readonly LLMApiSettingKeys _apiSettingKeys = new()
        {
            Parser = "parser",
            Chat = "chat",
        };

        private readonly LLMInputSettings _inputSettings = new()
        {
            UserInputDelimiter = "---USER INPUT---",
            FileNameDelimiter = "---FILE NAME---",
            FileContentDelimiter = "---FILE CONTENT---",
            RulesPromptDelimiter = "---RULES---"
        };

        private readonly Fixture _fix = new();
        private LMStudioMapper _mapper;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var generatorMock = new Mock<ILLMInputGenerator>();
            generatorMock.Setup(g => g.GenerateInput(It.IsAny<MessageDto>(), It.IsAny<string>()))
                .Returns("INPUT");
            generatorMock.Setup(g => g.GenerateInput(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("INPUT");

            var settingsChooserMock = new Mock<ILLMApiSettingsChooser>();
            settingsChooserMock.Setup(s => s.GetSettings(It.IsAny<string>()))
                .Returns(_apiSettings);

            var keyOptions = Options.Create(_apiSettingKeys);
            var inputOptions = Options.Create(_inputSettings);

            _mapper = new LMStudioMapper(
                    generatorMock.Object,
                    settingsChooserMock.Object,
                    keyOptions);
        }

        [Test]
        public void ToMessageDto_From_Response()
        {
            var chatId = _fix.Create<int>();
            var response = _fix.Create<LMStudioResponse>();
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
            var msg = _fix.Create<MessageDto>();
            var res = _mapper.ToRequest(msg, prevResponseId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Model, Is.EqualTo(_apiSettings.Model));
            Assert.That(res.Input, Is.Not.Null);
            Assert.That(res.Input, Is.Not.Empty);
            Assert.That(res.PreviousResponseId, Is.EqualTo(prevResponseId));
        }

        [Test]
        public void ToRequest_From_Text()
        {
            var text = _fix.Create<string>();
            var res = _mapper.ToRequest(text);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Model, Is.EqualTo(_apiSettings.Model));
            Assert.That(res.Input, Is.Not.Null);
            Assert.That(res.Input, Is.Not.Empty);
        }
    }
}
