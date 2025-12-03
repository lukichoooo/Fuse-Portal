using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioApi : ILMStudioApi
    {
        private readonly LMStudioSettings _settings;
        private readonly HttpClient _client;

        public LMStudioApi(HttpClient client, IOptions<LMStudioSettings> options)
        {
            _settings = options.Value;
            _client = client;
            _client.BaseAddress = new Uri(_settings.URL);
        }

        public async Task<LMStudioResponse> SendMessageAsync(LMStudioRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _client.PostAsync(_settings.ChatRoute, content);
            res.EnsureSuccessStatusCode();

            var response = await res.Content.ReadFromJsonAsync<LMStudioResponse>();
            return response!;
        }
    }

}

