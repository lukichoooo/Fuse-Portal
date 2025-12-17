using System.Text.Json;
using System.Text.Json.Serialization;
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
            IHtmlCleaner htmlCleaner,
            IValidJsonExtractor jsonExtractor,
            IOptions<LLMApiSettingKeys> keyOptions
            ) : IPortalParser
    {
        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly LLMApiSettingKeys _keySettings = keyOptions.Value;
        private readonly IHtmlCleaner _htmlCleaner = htmlCleaner;
        private readonly IValidJsonExtractor _jsonExtractor = jsonExtractor;

        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new NullableIntConverter(),
            },
        };

        public async Task<PortalParserResponseDto> ParsePortalHtml(string page)
        {
            page = _htmlCleaner.CleanHtml(page);

            LMStudioRequest lmStudioRequest = _mapper.ToRequest(
                   text: page,
                    rulesPrompt: rulesPrompt);

            LMStudioResponse response = await _api.SendMessageAsync(
                    lmStudioRequest,
                    _keySettings.Parser
                    );

            var portalText = _mapper.ToOutputText(response);

            var portalJson = _jsonExtractor.ExtractJsonObject(portalText);

            return JsonSerializer.Deserialize<PortalParserResponseDto>(portalJson, _serializerOptions)
                   ?? throw new LMStudioApiException("LMStudio returned empty response");
        }


        public class NullableIntConverter : JsonConverter<int?>
        {
            public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                    return reader.GetInt32();
                if (reader.TokenType == JsonTokenType.String)
                {
                    var s = reader.GetString();
                    if (string.IsNullOrWhiteSpace(s)) return null;
                    if (int.TryParse(s, out var val)) return val;
                }
                return null;
            }

            public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                    writer.WriteNumberValue(value.Value);
                else
                    writer.WriteNullValue();
            }
        }


        readonly string rulesPrompt = $@"
SYSTEM:
You extract data only. Do NOT guess or invent.

OUTPUT:
- Return ONE valid JSON object only.
- Must start with {{ and end with }}.
- No text outside JSON.
- JSON-escape all strings.
- Never use null.
- Missing values → """" or [].

DATES:
- ISO-8601 only: YYYY-MM-DDTHH:mm:ssZ
- Month 01–12 only.
- Valid day for month.
- If a date is unclear or invalid → REMOVE that schedule.
- Include schedules for each subject for the next 3 months from today {DateTime.UtcNow}.
- Ensure that all fields match the schema exactly. 'grade' should always be a number. If the grade is missing, use 0.

SCHEMA (EXACT, ALL KEYS REQUIRED):
{{
  ""subjects"": [
    {{
      ""name"": ""string"",
      ""grade"": 0,
      ""metadata"": ""string"",
      ""schedules"": [
        {{
          ""start"": ""YYYY-MM-DDTHH:mm:ssZ"",
          ""end"": ""YYYY-MM-DDTHH:mm:ssZ"",
          ""location"": ""string"",
          ""metadata"": ""string""
        }}
      ],
      ""lecturers"": [{{ ""name"": ""string"" }}],
      ""syllabuses"": [
        {{ ""name"": ""string"", ""content"": ""string"", ""metadata"": ""string"" }}
      ]
    }}
  ],
  ""metadata"": ""string""
}}

RULE:
- Never omit required keys.
- Empty data → empty array [].

OUTPUT JSON ONLY.
";

    }
}
