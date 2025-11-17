using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.User.Messaging
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "userPolicy")] // ili neka druga politika za ulogu korisnika
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // --- Slanje poruke ---
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> SendMessage([FromBody] SendMessageRequest request)
        {
            var message = await _messageService.SendMessageAsync(
                request.SenderId,
                request.ReceiverId,
                request.Content
            );

            return Ok(message);
        }

        // --- Dohvatanje svih konverzacija korisnika ---
        [HttpGet("conversations/{userId}")]
        public async Task<ActionResult<IEnumerable<ConversationDTO>>> GetUserConversations(long userId)
        {
            var conversations = await _messageService.GetUserConversationsAsync(userId);
            return Ok(conversations);
        }


        // --- Brisanje poruke ---
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(long messageId)
        {
            await _messageService.DeleteMessageAsync(messageId);
            return NoContent();
        }
    }

    // DTO koji se koristi za slanje poruke
    public class SendMessageRequest
    {
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string Content { get; set; }
    }

    // DTO koji se koristi za izmenu poruke
    public class EditMessageRequest
    {
        public string NewContent { get; set; }
    }
}
