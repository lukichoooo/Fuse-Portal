using System.Text;
using System.Text.Json;
using AutoFixture;
using Core.Dtos;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.Extensions.Logging;
using Moq;

namespace InfrastructureTests.LLM.LMStudio
{
    [TestFixture]
    public class LMStudioApiResponseStreamerTests
    {
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };


        private static LMStudioApiResponseStreamer CreateSut()
        {

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            };

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug);
                // .AddConsole();
            });

            var logger = loggerFactory.CreateLogger<LMStudioApiResponseStreamer>();

            return new LMStudioApiResponseStreamer(serializerOptions, logger);
        }

        private static string CreateDeltaResponse(string delta)
            => "event: response.output_text.delta\n" +
                $"data: {{\"type\":\"any\",\"delta\":\"{delta}\"}}\n";

        // private static string CreateCompletedResponse(string llmResponseJson)
        //     => "event: response.completed\n" +
        //        $"data: {{\"type\":\"any\",\"response\":{llmResponseJson}}}\n";
        //

        private static IEnumerable<List<string>> DeltaCases()
        {
            yield return new List<string> { "Hello" };
            yield return new List<string> { "e0wjf0fw}}", "Hi", "j9328r983" };
        }

        [TestCaseSource(nameof(DeltaCases))]
        public async Task ReadREsponseAsStreamAsync_Success(List<string> deltaInputs)
        {

            var content = "";
            foreach (var delta in deltaInputs)
            {
                content += CreateDeltaResponse(delta) + '\n';
            }
            var fix = new Fixture();
            var llmResponse = fix.Create<LMStudioResponse>();
            var llmResponseJson = JsonSerializer.Serialize(
                    llmResponse,
                    _serializerOptions);

            // content += CreateCompletedResponse(llmResponseJson);

            var response = new HttpResponseMessage
            {
                Content = new StringContent(content, Encoding.UTF8)
            };
            var sut = CreateSut();
            List<string> deltaResults = [];
            Func<string, Task> onRecieved = (string text) =>
            {
                deltaResults.Add(text);
                return Task.CompletedTask;
            };

            var result = await sut.ReadResponseAsStreamAsync(
                response,
                onRecieved
            );

            Assert.That(deltaResults, Is.EquivalentTo(deltaInputs));
            // Assert.That(llmResponse.Id, Is.EqualTo(result!.Id));
        }


    }
}
