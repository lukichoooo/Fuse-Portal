using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces.Convo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers
{
    // TODO: Default arguments dont work
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ChatController(
            IChatService service,
            IOptions<ControllerSettings> options
            ) : ControllerBase
    {
        private readonly IChatService _service = service;
        private readonly ControllerSettings _settings = options.Value;

        [HttpGet]
        public async Task<ActionResult<List<ChatDto>>> GetAllChatsPageAsync(
            [FromQuery] int? lastId,
            [FromQuery] int? pageSize
            )
            => Ok(await _service.GetAllChatsPageAsync(
                        lastId,
                        pageSize ?? _settings.DefaultPageSize));


        [HttpGet("{chatId}")]
        public async Task<ActionResult<ChatFullDto>> GetFullChatPageAsync(
            [FromRoute] int chatId,
            [FromQuery] int? firstMsgId,
            [FromQuery] int? pageSize
            )
            => Ok(await _service.GetChatWithMessagesPageAsync(
                        chatId,
                        firstMsgId,
                        pageSize ?? _settings.BigPageSize));

        [HttpPost]
        public async Task<ActionResult<ChatDto>> CreateNewChatAsync(
                [FromBody] CreateChatRequest request
                )
            => await _service.CreateNewChatAsync(request);

        [HttpDelete("messages/{msgId}")]
        public async Task<ActionResult<MessageDto>> DeleteMessageByIdAsync(
            [FromRoute] int msgId
            )
            => Ok(await _service.DeleteMessageByIdAsync(msgId));

        // TODO: Add cencelation Token
        [HttpPost("messages/text")]
        public async Task<ActionResult<MessageDto>> SendMessageAsync(
            [FromBody] MessageRequest messageRequest
            )
            => Ok(await _service.SendMessageAsync(messageRequest));


        // TODO: Add cencelation Token
        [HttpPost("messages/file")]
        public async Task<ActionResult<List<int>>> UploadFilesAsync(
                [FromForm] IFormFileCollection Files
                )
            => Ok(await _service.UploadFilesAsync(await HelperFilesToStream(Files)));


        // Helper

        private static async Task<List<FileUpload>> HelperFilesToStream(IFormFileCollection formFiles)
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
