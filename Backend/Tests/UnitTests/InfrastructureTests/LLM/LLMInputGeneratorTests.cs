using AutoFixture;
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
        private readonly Fixture _fix = new();
        private readonly LLMInputSettings _settings = new()
        {
            RulesPromptDelimiter = "---RULES---",
            UserInputDelimiter = "---USER INPUT---",
            FileNameDelimiter = "---FILE NAME---",
            FileContentDelimiter = "---FILE CONTENT---",
        };

        private ILLMInputGenerator _generator;

        [SetUp]
        public void BeforeEach()
        {
            var options = Options.Create(_settings);
            _generator = new LLMInputGenerator(options);
        }

        private void AssertFields(
                string res,
                bool hasRule = false,
                bool hasUserInput = false,
                bool hasFile = false,
                bool hasFileName = false)
        {
            Assert.Multiple(() =>
            {
                Assert.That(res.Contains(_settings.RulesPromptDelimiter), Is.EqualTo(hasRule));
                Assert.That(res.Contains(_settings.UserInputDelimiter), Is.EqualTo(hasUserInput));
                Assert.That(res.Contains(_settings.FileNameDelimiter), Is.EqualTo(hasFileName));
                Assert.That(res.Contains(_settings.FileContentDelimiter), Is.EqualTo(hasFile));
            });

        }

        [Test]
        public void GenerateInput_WithRules_ReturnsExpectedString()
        {
            var dto = _fix.Build<MessageDto>()
                .With(m => m.Files, [])
                .Create();

            const string rules = "Do this carefully";
            string res = _generator.GenerateInput(dto, rules);
            AssertFields(res, hasRule: true, hasUserInput: true);
        }

        [Test]
        public void GenerateInput_WithFiles_ReturnsExpectedString()
        {
            var files = _fix.CreateMany<FileDto>()
                .ToList();
            var dto = _fix.Build<MessageDto>()
                .With(m => m.Files, files)
                .Create();

            string res = _generator.GenerateInput(dto, null);
            AssertFields(res, hasUserInput: true, hasFile: true, hasFileName: true);
        }

        [Test]
        public void GenerateInput_WithoutRulesOrFiles_ReturnsUserInputOnly()
        {
            var dto = _fix.Build<MessageDto>()
                .With(m => m.Files, [])
                .Create();

            string res = _generator.GenerateInput(dto, null);
            AssertFields(res, hasUserInput: true);
        }

        [Test]
        public void GenerateInput_Html_NoRule()
        {
            var html = _fix.Create<string>();

            string res = _generator.GenerateInput(html, null);
            AssertFields(res, hasFile: true);
        }

        [Test]
        public void GenerateInput_Html_WithRule()
        {
            var html = _fix.Create<string>();
            var rule = _fix.Create<string>();

            string res = _generator.GenerateInput(html, rule);
            AssertFields(res, hasRule: true, hasFile: true);
        }
    }
}

