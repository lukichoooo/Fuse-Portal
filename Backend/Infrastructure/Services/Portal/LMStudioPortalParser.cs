using System.Text.Json;
using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Exceptions;
using Core.Interfaces.LLM.LMStudio;
using Core.Interfaces.Portal;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioPortalParser(
            ILMStudioApi api,
            ILMStudioMapper mapper,
            IOptions<LLMApiSettingKeys> keyOptions
            ) : IPortalParser
    {
        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly LLMApiSettingKeys _keySettings = keyOptions.Value;

        // TODO: write in another file
        const string rulesPrompt = @"
### System Role
You are a high-performance data extraction engine.
Your sole purpose is to parse raw HTML from a university portal and convert it into a strict JSON object.

### Response Constraints
* Output Format: RETURN ONLY JSON. Do not include markdown formatting.
* Strict Schema: Adhere exactly to the provided JSON structure.
* Data Types: Grade (int/null), Date (ISO 8601).
* Missing Data: Use [] for empty lists, null for missing scalars.

### Target JSON Structure
{
  ""subjects"": [
    {
      ""name"": ""string"",
      ""grade"": 0,
      ""metadata"": ""string"",
      ""schedules"": [{ ""start"": ""ISO"", ""end"": ""ISO"", ""location"": ""string"", ""metadata"": ""string"" }],
      ""lecturers"": [{ ""name"": ""string"" }],
      ""tests"": [{ ""name"": ""string"", ""content"": ""string"", ""date"": ""ISO"", ""metadata"": ""string"" }]
    }
  ],
  ""metadata"": ""string""
}";

        public async Task<PortalParserResponseDto> ParsePortalHtml(PortalParserRequestDto request)
        {
            LMStudioRequest lmStudioRequest = _mapper.ToRequest(
                   html: request.HtmlPage,
                    rulesPrompt: rulesPrompt);

            LMStudioResponse response = await _api.SendMessageAsync(
                    lmStudioRequest,
                    _keySettings.Parser
                    );

            var portalJson = _mapper.ToOutputText(response);
            return JsonSerializer.Deserialize<PortalParserResponseDto>(portalJson)
                   ?? throw new LMStudioApiException("LMStudio returned empty response");
        }
    }
}
