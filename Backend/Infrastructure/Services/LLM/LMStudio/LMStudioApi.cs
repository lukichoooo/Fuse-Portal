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

            _logger.LogInformation("content sending to LLM --- \n {}", json);

            var response = await _httpClient.PostAsync(settings.ChatRoute, content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("LMStudio returned {StatusCode}: {Body}", response.StatusCode, body);
                throw new LMStudioApiException(
                    $"LMStudio API returned {response.StatusCode}: {body}",
                    (int)response.StatusCode
                );
            }

            // Deserialize directly from stream
            var result = await response.Content.ReadFromJsonAsync<LMStudioResponse>(_serializerOptions)
                   ?? throw new LMStudioApiException("LMStudio returned empty response");

            _logger.LogInformation("Text from LLM --- \n {}", result.Output[0].Content[0].Text);

            return result;
        }


    }
}

