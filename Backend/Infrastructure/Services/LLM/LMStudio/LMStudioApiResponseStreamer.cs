using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Interfaces.LLM;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioApiResponseStreamer(
            JsonSerializerOptions serializerOptions,
            ILogger<LMStudioApiResponseStreamer> logger
            ) : ILLMApiResponseStreamer
    {

        private readonly JsonSerializerOptions _serializerOptions = serializerOptions;
        private readonly ILogger<LMStudioApiResponseStreamer> _logger = logger;

        public async Task<LMStudioResponse?> ReadResponseAsStreamAsync(
                HttpResponseMessage responseMessage,
                Func<string, Task>? onReceived)
        {
            await using var stream = await responseMessage.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            string? line;
            string? currentEvent = null;
            string? currentData = null;
            LMStudioStreamEvent? streamEvent = null;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                _logger.LogInformation("Stream from LMStudio --- \n {}", line);

                if (line.Length == 0)
                {
                    if (currentEvent != null && currentData != null)
                    {
                        streamEvent = await HandleEventAsync(
                                currentEvent,
                                currentData,
                                onReceived)
                            ?? streamEvent;
                    }

                    currentEvent = null;
                    currentData = null;
                    continue;
                }

                if (line.StartsWith("event:"))
                    currentEvent = line["event:".Length..].Trim();
                else if (line.StartsWith("data:"))
                    currentData = line["data:".Length..].Trim();
            }

            return streamEvent?.Response;
        }

        // Helper

        private async ValueTask<LMStudioStreamEvent?> HandleEventAsync(
                string evt,
                string data,
                Func<string, Task>? onReceived)
        {
            var streamEvent = JsonSerializer.Deserialize<LMStudioStreamEvent>(
                    data,
                    _serializerOptions);

            if (streamEvent == null)
            {
                _logger.LogWarning("Failed to deserialize stream event: {Data}", data);
                return null;
            }

            switch (evt)
            {
                case "response.output_text.delta":
                    if (!string.IsNullOrEmpty(streamEvent.Delta) && onReceived != null)
                        await onReceived(streamEvent.Delta);

                    break;

                case "response.completed":
                    return streamEvent;
            }

            return null;
        }
    }
}
