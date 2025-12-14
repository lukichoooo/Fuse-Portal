using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioApi : ILMStudioApi
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<LMStudioApi> _logger;
        private readonly ILLMApiSettingsChooser _apiSettingsChooser;
        private readonly HttpClient _httpClient;

        public LMStudioApi(
                ILogger<LMStudioApi> logger,
                JsonSerializerOptions serializerOptions,
                ILLMApiSettingsChooser apiSettingsChooser,
                HttpClient httpClient
                )
        {
            _logger = logger;
            _serializerOptions = serializerOptions;
            _apiSettingsChooser = apiSettingsChooser;
            _httpClient = httpClient;
        }


        public async Task<LMStudioResponse> SendMessageAsync(
                LMStudioRequest request,
                string settingsKey
                )
        {
            var settings = _apiSettingsChooser.GetSettings(settingsKey);
            _httpClient.BaseAddress = new Uri(settings.URL);
            _httpClient.Timeout = TimeSpan.FromMinutes(settings.TimeoutMins);

            var json = JsonSerializer.Serialize(request, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("content sending to LMStudio --- \n {}", json);

            var response = await _httpClient.PostAsync(settings.ChatRoute, content);
            await CheckUnsuccessfulResponseAsync(response);

            // Deserialize directly from stream
            var result = await response.Content.ReadFromJsonAsync<LMStudioResponse>(_serializerOptions)
                   ?? throw new LMStudioApiException("LMStudio returned empty response");

            _logger.LogInformation("Text from LMStudio --- \n {}", result.Output[0].Content[0].Text);

            return result;
        }


        public async Task<LMStudioResponse> SendMessageStreamingAsync(
                LMStudioRequest lmStudioRequest,
                string settingsKey,
                Action<string>? onReceived = null
                )
        {
            var settings = _apiSettingsChooser.GetSettings(settingsKey);
            _httpClient.BaseAddress = new Uri(settings.URL);
            _httpClient.Timeout = TimeSpan.FromMinutes(settings.TimeoutMins);
            lmStudioRequest.Stream = true;

            var json = JsonSerializer.Serialize(lmStudioRequest, _serializerOptions);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("content sending to LMStudio --- \n {}", json);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, settings.ChatRoute)
            {
                Content = requestContent
            };

            var response = await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead
            );

            await CheckUnsuccessfulResponseAsync(response);

            LMStudioResponse? completedResponse = await ReadResponseAsStreamAsync(response, onReceived);

            _logger.LogInformation("Completed Response from LMStudio --- \n {}",
                    completedResponse?.Output[0].Content[0].Text);

            return completedResponse
                ?? throw new LMStudioApiException("Completed Response Null");
        }

        // Helper Methods

        private async Task CheckUnsuccessfulResponseAsync(HttpResponseMessage? response)
        {
            if (response?.IsSuccessStatusCode == false)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("LMStudio returned {StatusCode}: {Body}", response.StatusCode, body);
                throw new LMStudioApiException(
                    $"LMStudio API returned {response.StatusCode}: {body}",
                    (int)response.StatusCode
                );
            }
        }

        private async Task<LMStudioResponse?> ReadResponseAsStreamAsync(HttpResponseMessage responseMessage, Action<string>? onReceived = null)
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
                    if (currentEvent is not null && currentData is not null)
                    {
                        streamEvent = HandleEvent(currentEvent, currentData, onReceived)
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

        private LMStudioStreamEvent? HandleEvent(string evt, string data, Action<string>? onReceived = null)
        {
            var streamEvent = JsonSerializer.Deserialize<LMStudioStreamEvent>(data, _serializerOptions);
            if (streamEvent is null)
                return null;

            switch (evt)
            {
                case "response.output_text.delta":
                    if (!string.IsNullOrEmpty(streamEvent.Delta))
                        onReceived?.Invoke(streamEvent.Delta);
                    break;

                case "response.completed":
                    return streamEvent;
            }

            return null;
        }


    }
}

