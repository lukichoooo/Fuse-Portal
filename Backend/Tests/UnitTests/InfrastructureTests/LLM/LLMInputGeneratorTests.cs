using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM;
using Infrastructure.Services.LLM;
using Microsoft.Extensions.Options;

namespace InfrastructureTests.LLM
{
    [TestFixture]
    public class LLMInputGeneratorTests
    {
        private readonly LLMInputSettings _settings = new()
        {
            RulesDelimiter = "---RULES---",
            UserInputDelimiter = "---USER INPUT---",
            FileNameDelimiter = "---FILE NAME---",
            FileContentDelimiter = "---FILE CONTENT---"
        };
        private ILLMInputGenerator _generator;

        [SetUp]
        public void BeforeEach()
        {
            var options = Options.Create(_settings);
            _generator = new LLMInputGenerator(options);
        }

        private void AssertFields(string res, bool hasRule, bool hasInput, bool hasFile)
        {
            Assert.Multiple(() =>
            {
                Assert.That(res.Contains(_settings.RulesDelimiter), Is.EqualTo(hasRule));
                Assert.That(res.Contains(_settings.UserInputDelimiter), Is.EqualTo(hasInput));
                Assert.That(res.Contains(_settings.FileNameDelimiter), Is.EqualTo(hasFile));
                Assert.That(res.Contains(_settings.FileContentDelimiter), Is.EqualTo(hasFile));
            });

        }

        [Test]
        public void GenerateInput_WithRules_ReturnsExpectedString()
        {
            var dto = new MessageDto()
            {
                Id = 1,
                Text = "Test",
                CreatedAt = DateTime.UtcNow,
                ChatId = 1,
                Files = []
            };

            const string rules = "Do this carefully";
            string res = _generator.GenerateInput(dto, rules);
            AssertFields(res, hasRule: true, hasInput: true, hasFile: false);
        }

        [Test]
        public void GenerateInput_WithFiles_ReturnsExpectedString()
        {
            var files = new List<FileDto>
            {
                new("fileName", "text"),
                new("fileName", "text")
            };
            var dto = new MessageDto()
            {
                Id = 1,
                Text = "Test",
                CreatedAt = DateTime.UtcNow,
                ChatId = 1,
                Files = files
            };
            string res = _generator.GenerateInput(dto);
            AssertFields(res, hasRule: false, hasInput: true, hasFile: true);
        }

        [Test]
        public void GenerateInput_WithoutRulesOrFiles_ReturnsUserInputOnly()
        {
            var dto = new MessageDto()
            {
                Id = 1,
                Text = "Test",
                CreatedAt = DateTime.UtcNow,
                ChatId = 1,
                Files = []
            };

            string res = _generator.GenerateInput(dto);
            AssertFields(res, hasRule: false, hasInput: true, hasFile: false);
        }
    }
}

