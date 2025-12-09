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

            var json = JsonSerializer.Serialize(request, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("sending to content LLM --- \n {json}", json);

            var res = await _client.PostAsync(settings.ChatRoute, content);

            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync();
                _logger.LogError("LMStudio returned {StatusCode}: {Body}", res.StatusCode, body);
                throw new LMStudioApiException(
                    $"LMStudio API returned {res.StatusCode}: {body}",
                    (int)res.StatusCode
                );
            }

            // Deserialize directly from stream
            return await res.Content.ReadFromJsonAsync<LMStudioResponse>(_serializerOptions)
                   ?? throw new LMStudioApiException("LMStudio returned empty response");
        }


    }

}

