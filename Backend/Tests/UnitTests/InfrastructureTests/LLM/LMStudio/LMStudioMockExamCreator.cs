using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM.LMStudio;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.LLM.LMStudio
{
    [TestFixture]
    public class LMStudioMockExamGeneratorTests
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
            ExamGenerator = "exam-gen-key"
        };

        private LMStudioMockExamGenerator CreateSut(
            ILMStudioApi api,
            ILMStudioMapper mapper)
        {
            var keyOptions = Options.Create(_settingKeys);
            return new(api, mapper, keyOptions);
        }

        [Test]
        public async Task GenerateExamAsync_Success()
        {
            // Arrange
            var syllabi = _fix.Create<string>();
            var apiResponse = _fix.Create<LMStudioResponse>();
            var expectedExamOutput = "MOCK EXAM\nSubject: Computer Science\n...";

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        _settingKeys.ExamGenerator))
                    .ReturnsAsync(apiResponse);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(apiResponse))
                .Returns(expectedExamOutput);

            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(lmsRequest);

            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            // Act
            var result = await sut.GenerateExamAsync(syllabi);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expectedExamOutput));
        }

        [Test]
        public async Task GenerateExamAsync_CallsApiWithCorrectSettings()
        {
            // Arrange
            var syllabi = _fix.Create<string>();
            var apiResponse = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        _settingKeys.ExamGenerator))
                    .ReturnsAsync(apiResponse);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(It.IsAny<LMStudioResponse>()))
                .Returns(_fix.Create<string>());

            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(lmsRequest);

            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            // Act
            await sut.GenerateExamAsync(syllabi);

            // Assert
            apiMock.Verify(a => a.SendMessageAsync(
                It.IsAny<LMStudioRequest>(),
                _settingKeys.ExamGenerator), Times.Once);
        }


        [Test]
        public async Task GenerateExamAsync_CallsMapperToOutputTextWithApiResponse()
        {
            // Arrange
            var syllabi = _fix.Create<string>();
            var apiResponse = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()))
                    .ReturnsAsync(apiResponse);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(apiResponse))
                .Returns(_fix.Create<string>());

            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(lmsRequest);

            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            // Act
            await sut.GenerateExamAsync(syllabi);

            // Assert
            mapperMock.Verify(m => m.ToOutputText(apiResponse), Times.Once);
        }

        [Test]
        public async Task GenerateExamAsync_ReturnsEmptyString_WhenMapperReturnsEmpty()
        {
            // Arrange
            var syllabi = _fix.Create<string>();
            var apiResponse = _fix.Create<LMStudioResponse>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()))
                    .ReturnsAsync(apiResponse);

            var mapperMock = new Mock<ILMStudioMapper>();
            mapperMock.Setup(m => m.ToOutputText(apiResponse))
                .Returns(string.Empty);

            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(lmsRequest);

            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            // Act
            var result = await sut.GenerateExamAsync(syllabi);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GenerateExamAsync_ThrowsException_WhenApiThrows()
        {
            // Arrange
            var syllabi = _fix.Create<string>();

            var apiMock = new Mock<ILMStudioApi>();
            apiMock.Setup(a => a.SendMessageAsync(
                        It.IsAny<LMStudioRequest>(),
                        It.IsAny<string>()))
                    .ThrowsAsync(new Exception("API error"));

            var mapperMock = new Mock<ILMStudioMapper>();
            var lmsRequest = _fix.Create<LMStudioRequest>();
            mapperMock.Setup(m => m.ToRequest(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(lmsRequest);

            var sut = CreateSut(apiMock.Object, mapperMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await sut.GenerateExamAsync(syllabi));
        }
    }
}
