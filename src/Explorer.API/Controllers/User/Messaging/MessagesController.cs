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
            try
            {
                var message = await _messageService.SendMessageAsync(
                    request.SenderId,
                    request.ReceiverId,
                    request.Content
                );

                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    inner2 = ex.InnerException?.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }

        }


        // --- Dohvatanje svih konverzacija korisnika ---
        [HttpGet("conversations/{userId}")]
        public async Task<ActionResult<IEnumerable<ConversationDTO>>> GetUserConversations(long userId)
        {
            var conversations = await _messageService.GetUserConversationsAsync(userId);
            return Ok(conversations);
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetConversationMessages(long conversationId)
        {
            var messages = await _messageService.GetConversationMessagesAsync(conversationId);
            return Ok(messages);
        }


        // --- Brisanje poruke ---
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(long messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found") || ex.Message.Contains("already deleted"))
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the message" });
            }
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
