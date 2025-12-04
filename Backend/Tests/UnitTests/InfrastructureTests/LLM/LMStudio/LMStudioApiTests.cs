using System.Net;
using System.Text.Json;
using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Exceptions;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace InfrastructureTests.LLM.LMStudio;

[TestFixture]
public class LMStudioApiTests
{
    private Fixture _fixture;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private Mock<ILogger<LMStudioApi>> _loggerMock;
    private Mock<IOptions<LMStudioApiSettings>> _optionsMock;
    private JsonSerializerOptions _serializerOptions;
    private LMStudioApiSettings _settings;

    private LMStudioApi _sut = null!;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        _loggerMock = new Mock<ILogger<LMStudioApi>>();

        _settings = _fixture.Build<LMStudioApiSettings>()
                            .With(x => x.URL, "http://localhost:1234")
                            .With(x => x.ChatRoute, "/v1/chat/completions")
                            .Create();

        _optionsMock = new Mock<IOptions<LMStudioApiSettings>>();
        _optionsMock.Setup(x => x.Value).Returns(_settings);

        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
    }

    private void CreateSut()
    {
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

        _sut = new LMStudioApi(
            httpClient,
            _optionsMock.Object,
            _loggerMock.Object,
            _serializerOptions
        );
    }

    [Test]
    public async Task SendMessageAsync_WhenApiCallSucceeds_ShouldSerializeToSnakeCaseAndReturnResponse()
    {
        CreateSut();
        var request = _fixture.Create<LMStudioRequest>();
        var expectedResponse = _fixture.Create<LMStudioResponse>();

        var responseJson = JsonSerializer.Serialize(expectedResponse, _serializerOptions);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson)
            })
            .Verifiable();

        var result = await _sut.SendMessageAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(expectedResponse.Id));

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                CheckContentIsSnakeCase(req.Content, request)
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public void SendMessageAsync_WhenApiReturnsError_ShouldLogAndThrowException()
    {
        CreateSut();
        var request = _fixture.Create<LMStudioRequest>();
        const string errorContent = "Internal Server Error";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent(errorContent)
            });

        var ex = Assert.ThrowsAsync<LMStudioApiException>(async () =>
            await _sut.SendMessageAsync(request));

        Assert.That(ex.Message, Does.Contain(errorContent));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("LMStudio returned")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void SendMessageAsync_WhenResponseIsSuccessButNull_ShouldThrowException()
    {
        CreateSut();
        var request = _fixture.Create<LMStudioRequest>();

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null")
            });

        var ex = Assert.ThrowsAsync<LMStudioApiException>(async () =>
            await _sut.SendMessageAsync(request));

        Assert.That(ex.Message, Is.EqualTo("LMStudio returned empty response"));
    }

    private static bool CheckContentIsSnakeCase(HttpContent? content, LMStudioRequest originalRequest)
    {
        if (content == null) return false;

        var jsonString = content.ReadAsStringAsync().Result;

        bool hasSnakeCaseKey = jsonString.Contains("\"previous_response_id\"");
        bool hasOriginalValue = jsonString.Contains(originalRequest.PreviousResponseId ?? "");

        return hasSnakeCaseKey && hasOriginalValue;
    }
}
