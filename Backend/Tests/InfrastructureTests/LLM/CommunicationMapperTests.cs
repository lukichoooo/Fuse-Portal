using Core.Interfaces.Communication;
using Infrastructure.Services;
using AutoFixture;
using Core.Dtos;
using Core.Entities;

namespace InfrastructureTests.LLM
{
    [TestFixture]
    public class CommunicationMapperTests
    {
        private readonly ICommunicationMapper _mapper = new CommunicationMapper();
        private readonly Fixture _fixture = new();

        private Message CreateMessageNoFiles()
            => _fixture.Build<Message>()
                .With(c => c.Chat, CreateChatNoMessages())
                .With(c => c.Files, [])
                .Create();

        private Chat CreateChatNoMessages()
            => _fixture.Build<Chat>()
                .With(c => c.Messages, [])
                .Create();

        [Test]
        public void ToChat_From_Dto()
        {
            var dto = _fixture.Create<ChatDto>();
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


        [Test]
        public void ToMessage_From_DtoWithFile()
        {
            var dto = _fixture.Build<MessageDto>()
                .With(d => d.FileToContent, new Dictionary<string, string> { { "name", "text" } })
                .Create();

            Message res = _mapper.ToMessage(dto);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(dto, res);
            Assert.That(res.Files, Is.Not.Null);
            Assert.That(res.Files
                    .ConvertAll(f => f.Name)
                    .OrderBy(x => x),
                    Is.EquivalentTo(dto.FileToContent?.Keys?
                        .OrderBy(x => x)
                        .ToList()));
        }


        [Test]
        public void ToMessage_From_DtoNoFile()
        {
            var dto = _fixture.Build<MessageDto>()
                .With(d => d.FileToContent, () => null)
                .Create();

            Message res = _mapper.ToMessage(dto);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(dto, res);
            Assert.That(res.Files, Is.Empty);
        }


        [Test]
        public void ToMessageDto_From_Message_WithFiles()
        {
            var msg = _fixture.Build<Message>()
                .With(m => m.Files, _fixture.Build<ChatFile>()
                        .With(f => f.Message, CreateMessageNoFiles())
                        .CreateMany()
                        .ToList())
                .With(m => m.Chat, CreateChatNoMessages())
                .Create();
            MessageDto res = _mapper.ToMessageDto(msg);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(msg, res);
            Assert.That(res.FileToContent!.Keys
                    .ToList()
                    .Order(),
                    Is.EquivalentTo(msg.Files
                        .ConvertAll(f => f.Name)
                        .Order()));
        }


        [Test]
        public void ToMessageDto_From_Message_NoFiles()
        {
            var msg = CreateMessageNoFiles();
            MessageDto res = _mapper.ToMessageDto(msg);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(msg, res);
            Assert.That(res.FileToContent, Is.Null);
        }

    }
}
