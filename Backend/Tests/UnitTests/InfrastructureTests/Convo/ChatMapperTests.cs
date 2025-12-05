using AutoFixture;
using Core.Dtos;
using Core.Interfaces.Convo;
using Core.Entities.Convo;
using Infrastructure.Services;

namespace InfrastructureTests.LLM
{
    [TestFixture]
    public class ChatMapperTests
    {
        private readonly IChatMapper _mapper = new ChatMapper();
        private readonly Fixture _globalFixture = new();

        [SetUp]
        public void BeforeEach()
        {
            _globalFixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        private Message CreateMessageNoFiles()
            => _globalFixture.Build<Message>()
                .With(c => c.Chat, CreateChatNoMessages())
                .With(c => c.Files, [])
                .Create();

        private Chat CreateChatNoMessages()
            => _globalFixture.Build<Chat>()
                .With(c => c.Messages, [])
                .Create();

        [Test]
        public void ToChat_From_Dto()
        {
            var dto = _globalFixture.Create<ChatDto>();
            Chat res = _mapper.ToChat(dto);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(dto, res);
        }

        [Test]
        public void ToChatFullDto_From_Chat()
        {
            var chat = CreateChatNoMessages();
            ChatFullDto res = _mapper.ToFullChatDto(chat);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(chat, res);

        }

        [Test]
        public void ToChatDto_From_Chat()
        {
            var chat = CreateChatNoMessages();
            ChatDto res = _mapper.ToChatDto(chat);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(chat, res);
        }


        [TestCase(0)]
        [TestCase(3)]
        public void ToMessage_From_Dto(int repeat)
        {
            var fixture = new Fixture() { RepeatCount = repeat };
            var dto = fixture.Build<MessageDto>()
                .With(d => d.Files, fixture.CreateMany<FileDto>().ToList())
                .Create();

            Message res = _mapper.ToMessage(dto);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(dto, res);
            Assert.That(res.Files, Is.Not.Null);
            Assert.That(res.Files
                    .ConvertAll(f => f.Name)
                    .Order(),
                    Is.EquivalentTo(dto.Files
                        .ConvertAll(f => f.Name)
                        .Order()
                        .ToList()));
        }


        [TestCase(0)]
        [TestCase(3)]
        public void ToMessageDto_From_Message(int repeat)
        {
            var fixture = new Fixture() { RepeatCount = repeat };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var msg = fixture.Build<Message>()
                .With(m => m.Files, fixture.Build<ChatFile>()
                        .With(f => f.Message, CreateMessageNoFiles())
                        .CreateMany()
                        .ToList())
                .With(m => m.Chat, CreateChatNoMessages())
                .Create();
            MessageDto res = _mapper.ToMessageDto(msg);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(msg, res);
            Assert.That(res.Files, Is.Not.Null);
            Assert.That(res.Files.ToList()
                    .ConvertAll(f => f.Name)
                    .Order(),
                            Is.EquivalentTo(msg.Files
                                .ConvertAll(f => f.Name).Order()));
        }

        [TestCase]
        public void ToMessageDto_From_ClientMessage()
        {
            var cm = _globalFixture.Create<ClientMessage>();
            var files = _globalFixture.CreateMany<FileDto>().ToList();

            MessageDto res = _mapper.ToMessageDto(cm, files);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(cm, res);
            Assert.That(res.Files, Is.Not.Null);
            Assert.That(res.Files.ToList()
                    .ConvertAll(f => f.Name)
                    .Order(),
                            Is.EquivalentTo(files
                                .ConvertAll(f => f.Name).Order()));
        }

        [Test]
        public void ToMessage_From_ClientMessage()
        {
            var cm = _globalFixture.Create<ClientMessage>();
            Message res = _mapper.ToMessage(cm);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(cm, res);
            Assert.That(res.Files, Is.Not.Null);
        }

        [Test]
        public void ToMessageDto_From_ClientMessage_WithFiles()
        {
            var cm = _globalFixture.Create<ClientMessage>();
            var fileDtos = _globalFixture.CreateMany<FileDto>()
                .ToList();
            MessageDto res = _mapper.ToMessageDto(cm, fileDtos);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(cm, res);
            Assert.That(res.Files, Is.Not.Null);
        }


        [Test]
        public void ToChatFile_From_FileDto()
        {
            var dto = _globalFixture.Create<FileDto>();
            ChatFile res = _mapper.ToChatFile(dto);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(dto, res);
        }


        [Test]
        public void ToFileDto_From_ChatFile()
        {
            var chatFile = _globalFixture.Build<ChatFile>()
                .With(f => f.Message, CreateMessageNoFiles())
                .Create();
            FileDto res = _mapper.ToFileDto(chatFile);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(res, chatFile);
        }
    }
}
