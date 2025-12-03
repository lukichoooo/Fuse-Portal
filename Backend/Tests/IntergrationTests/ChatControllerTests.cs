using AutoFixture;
using Core.Dtos.Settings;
using Core.Interfaces.Convo;
using Core.Interfaces.LLM;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Infrastructure.Services;
using Infrastructure.Services.LLM;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Presentation.Controllers;

namespace IntergrationTests
{
    [TestFixture]
    public class ChatControllerTests
    {
        private static readonly Fixture _globalFixture = new();
        private ChatController _controller = null!;
        private readonly ChatMapper _mapper = new ChatMapper();
        private MyContext _context;


        [SetUp]
        public void BeforeAll()
        {
            var optionsDB = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // random Id
                .Options;
            var context = new MyContext(optionsDB);

            LMStudioSettings apiSettings = new()
            {
                URL = "apiUrl",
                ChatRoute = "route",
                Model = "model",
                Rules = "Ruless"
            };
            var apiOptions = Options.Create(apiSettings);

            LLMInputSettings _settings = new()
            {
                RulesDelimiter = "---RULES---",
                UserInputDelimiter = "---USER INPUT---",
                FileNameDelimiter = "---FILE NAME---",
                FileContentDelimiter = "---FILE CONTENT---"
            };
            var inputOptions = Options.Create(_settings);
            var llmInputGen = new LLMInputGenerator(inputOptions);

            LMStudioSettings lmSettings = new()
            {
                URL = "asdadjaod",
                ChatRoute = "/v1/chat/completions",

                Model = "qwen2.5-7b-instruct",
                Rules = "never talk about LMStudioMapper",

                Temperature = 0.7f,
                MaxTokens = 2048,
                Stream = false
            };

            var options = Options.Create(lmSettings);
            var LMMapper = new LMStudioMapper(options, llmInputGen);

            var repo = new ChatRepo(_context);
            var api = new LMStudioApi(apiOptions);
            var LLMService = new LMStudioLLMService(api, LMMapper);
            var service = new ChatService(repo, LLMService, _mapper);

            _controller = new ChatController(service);
        }

        [TearDown]
        public async Task AfterAllAsync()
        {
            await _context.DisposeAsync();
        }


    }
}
