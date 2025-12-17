using System.Text.Json;
using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM.LMStudio;
using Core.Interfaces.Portal;
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
            HtmlCleaner cleaner = new();
            var jsonExtractorMock = new Mock<IValidJsonExtractor>();
            jsonExtractorMock.Setup(e => e.ExtractJsonObject(It.IsAny<string>()))
                .Returns((string s) => s);

            var keyOptions = Options.Create(_settingKeys);
            return new(
                    api,
                    mapper,
                    cleaner,
                    jsonExtractorMock.Object,
                    keyOptions);
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
            Assert.That(res.Metadata, Is.EqualTo(parserResponseDto.Metadata));
        }


        [Test]
        public async Task ParsePortalHtml_EmptyGrade_DoesNotThrow()
        {
            var apiResponse = _fix.Create<LMStudioResponse>();

            var brokenJson = @"{
            ""subjects"": [
                { ""name"": ""Test Subject"", ""grade"": """", ""metadata"": ""meta"", ""schedules"": [], ""lecturers"": [], ""syllabuses"": [] }
            ],
            ""metadata"": ""meta""
        }";

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>(), It.IsAny<string>()))
                   .ReturnsAsync(apiResponse);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(apiResponse)).Returns(brokenJson);

            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(It.IsAny<MessageDto>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(lmsRequest);

            var request = _fix.Create<string>();
            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            Assert.DoesNotThrowAsync(async () => await sut.ParsePortalHtml(request));
        }

        [Test]
        public async Task ParsePortalHtml_ExtraFields_IgnoresExtras()
        {
            var apiResponse = _fix.Create<LMStudioResponse>();

            var extraJson = @"{
            ""subjects"": [
                { ""name"": ""Extra"", ""grade"": 5, ""metadata"": ""meta"", ""foo"": ""bar"", ""schedules"": [], ""lecturers"": [], ""syllabuses"": [] }
            ],
            ""metadata"": ""meta"",
            ""extraField"": ""ignoreMe""
        }";

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>(), It.IsAny<string>()))
                   .ReturnsAsync(apiResponse);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(apiResponse)).Returns(extraJson);

            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(It.IsAny<MessageDto>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(lmsRequest);

            var request = _fix.Create<string>();
            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            var res = await sut.ParsePortalHtml(request);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Subjects[0].Name, Is.EqualTo("Extra"));
        }

        [Test]
        public async Task ParsePortalHtml_MissingOptionalFields_DoesNotThrow()
        {
            var apiResponse = _fix.Create<LMStudioResponse>();

            var missingFieldsJson = @"{
            ""subjects"": [
                { ""name"": ""NoSchedules"", ""grade"": 0, ""metadata"": ""meta"" }
            ],
            ""metadata"": ""meta""
        }";

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(It.IsAny<LMStudioRequest>(), It.IsAny<string>()))
                   .ReturnsAsync(apiResponse);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(apiResponse)).Returns(missingFieldsJson);

            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(It.IsAny<MessageDto>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(lmsRequest);

            var request = _fix.Create<string>();
            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            Assert.DoesNotThrowAsync(async () => await sut.ParsePortalHtml(request));
        }

    }
}
