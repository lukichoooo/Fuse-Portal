using System.Text.Json;
using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM.LMStudio;
using Infrastructure.Services.LLM.LMStudio;
using Infrastructure.Services.Portal;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.LLM.LMStudio
{
    [TestFixture]
    public class LMStudioPortalParserTests
    {
        private readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        private readonly LLMApiSettingKeys _settingKeys = new()
        {
            Chat = "chatyy",
            Parser = "parserr",
        };

        private LMStudioPortalParser CreateSut(
            ILMStudioApi api,
            ILMStudioMapper mapper
                )
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            };
            HtmlCleaner cleaner = new();

            var keyOptions = Options.Create(_settingKeys);
            return new(api, mapper, keyOptions, cleaner, jsonSerializerOptions);
        }


        [Test]
        public async Task ParsePortalHtml_Success()
        {
            var apiResponse = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()))
                    .ReturnsAsync(apiResponse);

            var parserResponseDto = _fix.Create<PortalParserResponseDto>();
            var outputText = JsonSerializer.Serialize(parserResponseDto);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(apiResponse))
                .Returns(outputText);
            var lmsRequest = _fix.Create<LMStudioRequest>();

            mapperMock.Setup(m => m.ToRequest(
                        It.IsAny<MessageDto>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(lmsRequest);

            var request = _fix.Create<string>();
            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            var res = await sut.ParsePortalHtml(request);

            Assert.That(res, Is.Not.Null);
        }


    }
}
