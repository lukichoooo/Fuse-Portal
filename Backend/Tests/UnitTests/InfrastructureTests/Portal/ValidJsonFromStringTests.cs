using Core.Interfaces.Portal;
using Infrastructure.Services.Portal;

namespace Infrastructure.Tests.Services.Portal
{
    [TestFixture]
    public class ValidJsonFromStringTests
    {
        private IValidJsonExtractor _extractor;

        [SetUp]
        public void Setup()
        {
            _extractor = new ValidJsonExtractor();
        }

        [Test]
        public void Extracts_Simple_Json_Object()
        {
            var input = @"{""a"":1}";

            var result = _extractor.ExtractJsonObject(input);

            Assert.That(result, Is.EqualTo(@"{""a"":1}"));
        }

        [Test]
        public void Extracts_Json_With_Text_Before_And_After()
        {
            var input = @"prefix {""a"":1} suffix";

            var result = _extractor.ExtractJsonObject(input);

            Assert.That(result, Is.EqualTo(@"{""a"":1}"));
        }

        [Test]
        public void Extracts_Nested_Json_Object()
        {
            var input = @"prefix {""a"":{""b"":2,""c"":{""d"":3}}} suffix";

            var result = _extractor.ExtractJsonObject(input);

            Assert.That(result, Is.EqualTo(@"{""a"":{""b"":2,""c"":{""d"":3}}}"));
        }

        [Test]
        public void Extracts_First_Complete_Json_Object_Only()
        {
            var input = @"{""a"":1}{""b"":2}";

            var result = _extractor.ExtractJsonObject(input);

            Assert.That(result, Is.EqualTo(@"{""a"":1}"));
        }

        [Test]
        public void Throws_When_No_Json_Object()
        {
            var input = @"no json here";

            Assert.That(() => _extractor.ExtractJsonObject(input),
                        Throws.InvalidOperationException
                              .With.Message.EqualTo("No valid JSON object found."));
        }

        [Test]
        public void Throws_When_Json_Is_Unbalanced()
        {
            var input = @"{""a"":1";

            Assert.That(() => _extractor.ExtractJsonObject(input),
                        Throws.InvalidOperationException
                              .With.Message.EqualTo("No valid JSON object found."));
        }

        [Test]
        public void Handles_Empty_Object()
        {
            var input = @"text {} text";

            var result = _extractor.ExtractJsonObject(input);

            Assert.That(result, Is.EqualTo("{}"));
        }

        [Test]
        public void Handles_Multiline_Json()
        {
            var input = @"
                some text
                {
                    ""a"": 1,
                    ""b"": {
                        ""c"": 2
                    }
                }
                more text";

            var result = _extractor.ExtractJsonObject(input);

            Assert.That(result.Trim(), Is.EqualTo(@"
                {
                    ""a"": 1,
                    ""b"": {
                        ""c"": 2
                    }
                }".Trim()));
        }
    }
}

