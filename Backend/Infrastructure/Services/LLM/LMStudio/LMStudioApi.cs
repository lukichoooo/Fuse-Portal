using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Dtos.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioApi
    {
        private readonly LMStudioSettings _settings;
        private readonly HttpClient _client;
        private const string _contentType = "application/json";

        public LMStudioApi(IOptions<LMStudioSettings> options)
        {
            _settings = options.Value;
            _client = new HttpClient { BaseAddress = new Uri(_settings.URL) };
        }

        public async Task<LMStudioResponse> SendMessageAsync(LMStudioRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, _contentType);

            HttpResponseMessage res = await _client.PostAsync(_settings.ChatRoute, content);
            res.EnsureSuccessStatusCode();

            string responseJson = await res.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<LMStudioResponse>(responseJson);

            return response!;
        }

    }
}

