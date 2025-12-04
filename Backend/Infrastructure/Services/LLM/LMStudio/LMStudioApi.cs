using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Exceptions;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioApi : ILMStudioApi
    {
        private readonly ILogger<LMStudioApi> _logger;
        private readonly LMStudioApiSettings _settings;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        public LMStudioApi(
                HttpClient client,
                IOptions<LMStudioApiSettings> options,
                ILogger<LMStudioApi> logger,
                JsonSerializerOptions serializerOptions
                )
        {
            _settings = options.Value;
            _client = client;
            _client.BaseAddress = new Uri(_settings.URL);
            _logger = logger;
            _serializerOptions = serializerOptions;
        }


        public async Task<LMStudioResponse> SendMessageAsync(LMStudioRequest request)
        {
            var json = JsonSerializer.Serialize(request, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _logger.LogInformation("sending to content LLM --- \n {json}", json);

            var res = await _client.PostAsync(_settings.ChatRoute, content);

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

