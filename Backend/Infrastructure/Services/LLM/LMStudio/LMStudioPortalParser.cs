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
            IOptions<LLMApiSettingKeys> keyOptions,
            IHtmlCleaner htmlCleaner,
            JsonSerializerOptions serializerOptions
            ) : IPortalParser
    {
        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly LLMApiSettingKeys _keySettings = keyOptions.Value;
        private readonly JsonSerializerOptions _serializerOptions = serializerOptions;
        private readonly IHtmlCleaner _htmlCleaner = htmlCleaner;


        public async Task<PortalParserResponseDto> ParsePortalHtml(string HtmlPage)
        {
            HtmlPage = _htmlCleaner.CleanHtml(HtmlPage);

            LMStudioRequest lmStudioRequest = _mapper.ToRequest(
                   text: HtmlPage,
                    rulesPrompt: rulesPrompt);

            LMStudioResponse response = await _api.SendMessageAsync(
                    lmStudioRequest,
                    _keySettings.Parser
                    );

            var portalJson = _mapper.ToOutputText(response);

            portalJson = portalJson.Trim();
            if (portalJson.StartsWith("```")) portalJson = portalJson[3..];
            if (portalJson.EndsWith("```")) portalJson = portalJson[..^3];

            return JsonSerializer.Deserialize<PortalParserResponseDto>(portalJson, _serializerOptions)
                   ?? throw new LMStudioApiException("LMStudio returned empty response");
        }


        readonly string rulesPrompt = @"
### System Role
You are a high-performance data extraction engine.
Your sole purpose is to parse raw HTML from a university portal and convert it into a strict JSON object.

### Response Constraints
* Output must be a single valid JSON object starting with { and ending with }.
* Do NOT include any explanations, text, headers, or markdown.
* Escape all special characters in string fields (including newlines \n and quotes \"").
* Follow the schema exactly.

### Target JSON Structure
{
  ""subjects"": [
    {
      ""name"": ""string"",
      ""grade"": 0,
      ""metadata"": ""string"",
      ""schedules"": [{ ""start"": ""ISO"", ""end"": ""ISO"", ""location"": ""string"", ""metadata"": ""string"" }],
      ""lecturers"": [{ ""name"": ""string"" }],
      ""syllabuses"": [{ ""name"": ""string"", ""content"": ""string"", ""metadata"": ""string"" }]
    }
  ],
  ""metadata"": ""string""
}

### IMPORTANT
Output must start with ""{"" and end with ""}"" NOTHING else.
"
+
$"* Include schedules for each subject for the next 5 months from today {DateTime.UtcNow}.";

    }
}
