using Core.Dtos;
using Core.Interfaces.Convo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ChatController(IChatService service) : ControllerBase
    {
        private readonly IChatService _service = service;

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
        public async Task<ActionResult<MessageDto>> SendMessageAsync( // TODO: Add cencelation Token
            [FromBody] ClientMessage message)
            => Ok(await _service.SendMessageAsync(message));


        [HttpPost("messages/file")]
        public async Task<ActionResult<MessageDto>> SendMessageWithFileAsync( // TODO: Add cencelation Token
                [FromBody] ClientMessage message,
                IFormFileCollection files
                )
        {
            return Ok(await _service.SendMessageAsync(message));
        }


        [HttpDelete("messages/{msgId}")]
        public async Task<ActionResult<MessageDto>> RemoveMessageByIdAsync(
            [FromRoute] int msgId)
            => Ok(await _service.DeleteMessageByIdAsync(msgId));
    }
}
