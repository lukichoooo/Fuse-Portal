using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AICommunicationController(ICommunicationService service) : ControllerBase
    {
        private readonly ICommunicationService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<ChatDto>>> GetAllChatsPageAsync(
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16)
            => Ok(await _service.GetAllChatsPageAsync(lastId, pageSize));


        [HttpGet("{chatId}")]
        public async Task<ActionResult<ChatFullDto>> GetFullChatPageAsync(
            [FromRoute] int chatId,
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16)
            => Ok(await _service.GetFullChatPageAsync(chatId, lastId, pageSize));


        [HttpPost("messages/text")]
        public async Task<ActionResult<MessageDto>> SendMessageAsync(
            [FromBody] MessageDto message)
            => Ok(await _service.SendMessageAsync(message));


        [HttpPost("messages/file")]
        public async Task<ActionResult<MessageDto>> SendMessageWithFileAsync(
                [FromBody] MessageWithFileDto dto)
            => Ok(await _service.SendMessageWithFileAsync(dto));


        [HttpDelete("messages/{msgId}")]
        public async Task<ActionResult<MessageDto>> RemoveMessageByIdAsync(
            [FromRoute] int msgId)
            => Ok(await _service.RemoveMessageByIdAsync(msgId));
    }
}
