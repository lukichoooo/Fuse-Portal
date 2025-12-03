using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.JsonPolicies;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioApi : ILMStudioApi
    {
        private readonly ILogger<LMStudioApi> _logger;
        private readonly LMStudioApiSettings _settings;
        private readonly HttpClient _client;

        // TODO: seperate
        private readonly JsonSerializerOptions options = new();

        public LMStudioApi(HttpClient client, IOptions<LMStudioApiSettings> options, ILogger<LMStudioApi> logger)
        {
            _settings = options.Value;
            _client = client;
            _client.BaseAddress = new Uri(_settings.URL);
            _logger = logger;
        }

        public async Task<LMStudioResponse> SendMessageAsync(LMStudioRequest request)
        {
            try
            {
                options.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _logger.LogInformation($"sending to LLM content --- \n{json}");

                var res = await _client.PostAsync(_settings.ChatRoute, content);
                // res.EnsureSuccessStatusCode();
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync();
                    _logger.LogError("LMStudio returned {StatusCode}: {Body}", res.StatusCode, body);
                    res.EnsureSuccessStatusCode();
                }

                var response = await res.Content.ReadFromJsonAsync<LMStudioResponse>();
                return response!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to LMStudio API");
                throw;
            }
        }

    }

}

