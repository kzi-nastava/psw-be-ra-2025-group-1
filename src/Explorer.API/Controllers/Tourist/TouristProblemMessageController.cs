using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/problem-messages")]
[ApiController]
public class TouristProblemMessageController : ControllerBase
{
    private readonly IProblemMessageService _problemMessageService;
    private readonly INotificationService _notificationService;
    private readonly IProblemService _problemService;

    public TouristProblemMessageController(
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
            var touristId = User.PersonId();
            var message = _problemMessageService.AddMessage(dto.ProblemId, touristId, dto.Content);

            var problem = _problemService.Get(dto.ProblemId, touristId);
            _notificationService.Create(
                problem.AuthorId,
                $"New message from tourist on problem #{dto.ProblemId}",
                NotificationTypeDto.ProblemReportMessage,
                dto.ProblemId
            );

            return Ok(message);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
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
            var touristId = User.PersonId();
            _problemService.Get(problemId, touristId);
            
            var messages = _problemMessageService.GetMessagesByProblemId(problemId);
            return Ok(messages);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
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
