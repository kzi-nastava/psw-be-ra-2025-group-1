using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/admin/problem-messages")]
[ApiController]
public class AdminProblemMessageController : ControllerBase
{
    private readonly IProblemMessageService _problemMessageService;
    private readonly IProblemService _problemService;

    public AdminProblemMessageController(
        IProblemMessageService problemMessageService,
        IProblemService problemService)
    {
        _problemMessageService = problemMessageService;
        _problemService = problemService;
    }

    [HttpGet("{problemId:long}")]
    public ActionResult<List<ProblemMessageDto>> GetMessages(long problemId)
    {
        try
        {
            var messages = _problemMessageService.GetMessagesByProblemId(problemId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public ActionResult<ProblemMessageDto> AddMessage([FromBody] AddProblemMessageDto dto)
    {
        try
        {
            var adminId = User.UserId();
            var message = _problemMessageService.AddMessage(dto.ProblemId, adminId, dto.Content, isAdmin: true);
            return Ok(message);
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
}
