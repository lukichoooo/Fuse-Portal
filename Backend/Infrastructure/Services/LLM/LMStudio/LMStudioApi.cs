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
        private readonly HttpClient _client;
        private readonly ILLMApiSettingsChooser _apiSettingsChooser;

        public LMStudioApi(
                HttpClient client,
                ILogger<LMStudioApi> logger,
                JsonSerializerOptions serializerOptions,
                ILLMApiSettingsChooser apiSettingsChooser
                )
        {
            _client = client;
            _logger = logger;
            _serializerOptions = serializerOptions;
            _apiSettingsChooser = apiSettingsChooser;
        }


        public async Task<LMStudioResponse> SendMessageAsync(
                LMStudioRequest request,
                string settingsKey
                )
        {
            var settings = _apiSettingsChooser.GetSettings(settingsKey);
            _client.BaseAddress = new Uri(settings.URL);
            _client.Timeout = TimeSpan.FromMinutes(settings.TimeoutMins);

            var json = JsonSerializer.Serialize(request, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("sending to content LLM --- \n {}", json);

            var result = await _client.PostAsync(settings.ChatRoute, content);

            if (!result.IsSuccessStatusCode)
            {
                var body = await result.Content.ReadAsStringAsync();
                _logger.LogError("LMStudio returned {StatusCode}: {Body}", result.StatusCode, body);
                throw new LMStudioApiException(
                    $"LMStudio API returned {result.StatusCode}: {body}",
                    (int)result.StatusCode
                );
            }

            // Deserialize directly from stream
            var response = await result.Content.ReadFromJsonAsync<LMStudioResponse>(_serializerOptions)
                   ?? throw new LMStudioApiException("LMStudio returned empty response");

            _logger.LogInformation("Text from LLM --- \n {}", response.Output[0].Content[0].Text);

            return response;
        }

    }

}

