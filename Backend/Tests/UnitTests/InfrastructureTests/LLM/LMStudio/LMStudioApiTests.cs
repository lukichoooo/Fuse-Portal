using System.Net;
using System.Text.Json;
using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Dtos.Settings.Infrastructure;
using Core.Exceptions;
using Core.Interfaces.LLM;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace InfrastructureTests.LLM.LMStudio;

[TestFixture]
public class LMStudioApiTests
{
    private Fixture _fixture;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private Mock<ILogger<LMStudioApi>> _loggerMock;
    private JsonSerializerOptions _serializerOptions;
    private LLMApiSettings _apiSettings;
    private LLMApiSettingKeys _settingKeys;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        _loggerMock = new Mock<ILogger<LMStudioApi>>();

        _apiSettings = _fixture.Build<LLMApiSettings>()
                            .With(x => x.URL, "http://localhost:1234")
                            .With(x => x.ChatRoute, "/v1/chat/completions")
                            .Create();
        _settingKeys = _fixture.Create<LLMApiSettingKeys>();

        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
    }

    private LMStudioApi CreateSut(
            ILLMApiResponseStreamer? responseStreamer = null)
    {
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var settingsChooserMock = new Mock<ILLMApiSettingsChooser>();
        settingsChooserMock.Setup(s => s.GetSettings(It.IsAny<string>()))
            .Returns(_apiSettings);

        responseStreamer ??= new Mock<ILLMApiResponseStreamer>().Object;

        return new(
            _loggerMock.Object,
            _serializerOptions,
            settingsChooserMock.Object,
            httpClient,
            responseStreamer
        );
    }



    [Test]
    public async Task SendMessageAsync_Success()
    {
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
            });
        var sut = CreateSut();

        var result = await sut.SendMessageAsync(request, _settingKeys.Chat);

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
        var sut = CreateSut();

        var ex = Assert.ThrowsAsync<LMStudioApiException>(async () =>
            await sut.SendMessageAsync(request, _settingKeys.Chat));

        Assert.That(ex.Message, Does.Contain(errorContent));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!
                    .Contains("LMStudio returned")),
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
        var sut = CreateSut();

        Assert.ThrowsAsync<LMStudioApiException>(async () =>
            await sut.SendMessageAsync(request, _settingKeys.Chat));
    }


    [Test]
    public async Task SendMessageStreamingAsyncAsync_Success()
    {
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
            });

        bool onRecievedCalled = false;
        Action<string> onRecieved = (string _) => onRecievedCalled = true;

        var streamerMock = new Mock<ILLMApiResponseStreamer>();
        streamerMock.Setup(s => s.ReadResponseAsStreamAsync(
                    It.IsAny<HttpResponseMessage>(),
                    onRecieved
                    ))
            .ReturnsAsync((HttpResponseMessage _, Action<string> onRecieved) =>
                    {
                        onRecieved.Invoke("message");
                        return expectedResponse;
                    });
        var sut = CreateSut(streamerMock.Object);

        var result = await sut.SendMessageWithStreamingAsync(
                request,
                _settingKeys.Chat,
                onRecieved);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(expectedResponse.Id));
        Assert.That(onRecievedCalled, Is.True);

        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    CheckContentIsSnakeCase(req.Content, request)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }


    [Test]
    public async Task SendMessageStreamingAsyncAsync_CompletedNull_Throws()
    {
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
            });

        bool onRecievedCalled = false;
        Action<string> onRecieved = (string _) => onRecievedCalled = true;

        var streamerMock = new Mock<ILLMApiResponseStreamer>();
        streamerMock.Setup(s => s.ReadResponseAsStreamAsync(
                    It.IsAny<HttpResponseMessage>(),
                    onRecieved
                    ))
            .ReturnsAsync((HttpResponseMessage _, Action<string> onRecieved) =>
                    {
                        onRecieved.Invoke("message");
                        return null;
                    });
        var sut = CreateSut(streamerMock.Object);

        Assert.ThrowsAsync<LMStudioApiException>(async () =>
                await sut.SendMessageWithStreamingAsync(
                request,
                _settingKeys.Chat,
                onRecieved));

        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    CheckContentIsSnakeCase(req.Content, request)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }


    [Test]
    public async Task SendMessageStreamingAsyncAsync_UnsuccessfulResponse_Throws()
    {
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
                StatusCode = HttpStatusCode.InternalServerError,
            });

        var sut = CreateSut();

        Assert.ThrowsAsync<LMStudioApiException>(async () =>
                await sut.SendMessageWithStreamingAsync(
                request,
                _settingKeys.Chat,
                null));

        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    CheckContentIsSnakeCase(req.Content, request)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }




    // Helper

    private static bool CheckContentIsSnakeCase(HttpContent? content, LMStudioRequest originalRequest)
    {
        if (content == null) return false;

        var jsonString = content.ReadAsStringAsync().Result;

        bool hasSnakeCaseKey = jsonString.Contains("\"previous_response_id\"");
        bool hasOriginalValue = jsonString.Contains(originalRequest.PreviousResponseId ?? "");

        return hasSnakeCaseKey && hasOriginalValue;
    }


}
