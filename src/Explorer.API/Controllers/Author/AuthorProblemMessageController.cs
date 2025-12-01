using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/problem-messages")]
[ApiController]
public class AuthorProblemMessageController : ControllerBase
{
    private readonly IProblemMessageService _problemMessageService;
    private readonly INotificationService _notificationService;
    private readonly IProblemService _problemService;

    public AuthorProblemMessageController(
        IProblemMessageService problemMessageService,
        INotificationService notificationService,
        IProblemService problemService)
    {
        _problemMessageService = problemMessageService;
        _notificationService = notificationService;
        _problemService = problemService;
    }

    [HttpPost]
    public ActionResult<ProblemMessageDto> AddMessage([FromBody] AddProblemMessageDto dto)
    {
        try
        {
            var authorId = User.PersonId();
            var message = _problemMessageService.AddMessage(dto.ProblemId, authorId, dto.Content);

            var problem = _problemService.Get(dto.ProblemId, authorId);
            _notificationService.Create(
                problem.CreatorId,  
                $"New message from tour author on problem #{dto.ProblemId}",
                NotificationTypeDto.ProblemReportMessage,
                dto.ProblemId
            );

            return Ok(message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{problemId:long}")]
    public ActionResult<List<ProblemMessageDto>> GetMessages(long problemId)
    {
        try
        {
            var authorId = User.PersonId();
            _problemService.Get(problemId, authorId);
            
            var messages = _problemMessageService.GetMessagesByProblemId(problemId);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
