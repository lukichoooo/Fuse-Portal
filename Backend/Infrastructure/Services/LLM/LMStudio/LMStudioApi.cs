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
        private readonly ILLMApiResponseStreamer _responseStreamer;

        public LMStudioApi(
                ILogger<LMStudioApi> logger,
                JsonSerializerOptions serializerOptions,
                ILLMApiSettingsChooser apiSettingsChooser,
                HttpClient httpClient,
                ILLMApiResponseStreamer responseStreamer
                )
        {
            _logger = logger;
            _serializerOptions = serializerOptions;
            _apiSettingsChooser = apiSettingsChooser;
            _httpClient = httpClient;
            _responseStreamer = responseStreamer;
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


        public async Task<LMStudioResponse> SendMessageWithStreamingAsync(
                LMStudioRequest lmStudioRequest,
                string settingsKey,
                Func<string, Task>? onReceived)
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

            LMStudioResponse? completedResponse =
                await _responseStreamer.ReadResponseAsStreamAsync(response, onReceived);

            _logger.LogInformation("Completed Response from LMStudio --- \n {}",
                    completedResponse?.ToString());

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


    }
}

