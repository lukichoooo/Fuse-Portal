using Core.Dtos;
using Core.Interfaces.Convo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    // TODO: Default arguments dont work
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

        [HttpPost("{chatName}")]
        public async Task<ActionResult<ChatDto>> CreateNewChat(
                [FromRoute] string chatName = "New Chat"
                )
            => await _service.CreateNewChat(chatName);


        [HttpDelete("messages/{msgId}")]
        public async Task<ActionResult<MessageDto>> RemoveMessageByIdAsync(
            [FromRoute] int msgId)
            => Ok(await _service.DeleteMessageByIdAsync(msgId));

        // TODO: Add cencelation Token
        [HttpPost("messages/text")]
        public async Task<ActionResult<MessageDto>> SendMessageAsync(
            [FromBody] MessageRequest messageRequest)
            => Ok(await _service.SendMessageAsync(messageRequest));


        // TODO: Add cencelation Token
        [HttpPost("messages/file")]
        public async Task<ActionResult<List<int>>> UploadFilesForMessage(
                [FromForm] IFormFileCollection Files)
            => Ok(await _service.UploadFilesForMessageAsync(await FilesToStream(Files)));



        // Helper

        private static async Task<List<FileUpload>> FilesToStream(IFormFileCollection formFiles)
        {
            var result = new List<FileUpload>(formFiles.Count);
            foreach (var file in formFiles)
            {
                if (file.Length == 0)
                    continue;
                result.Add(new FileUpload(file.FileName, file.OpenReadStream()));
            }
            return result;
        }

    }
}
