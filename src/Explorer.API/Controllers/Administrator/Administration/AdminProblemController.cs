using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/admin/problems")]
[ApiController]
public class AdminProblemController : ControllerBase
{
    private readonly IProblemService _problemService;
    private readonly ITourService _tourService;

    public AdminProblemController(IProblemService problemService, ITourService tourService)
    {
        _problemService = problemService;
        _tourService = tourService;
    }

    [HttpGet]
    public ActionResult<PagedResult<ProblemDto>> GetAllProblems([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _problemService.GetPaged(page, pageSize);
        return Ok(result);
    }

    [HttpGet("overdue")]
    public ActionResult<List<ProblemDto>> GetOverdueProblems([FromQuery] int days = 5)
    {
        var problems = _problemService.GetUnresolvedOlderThan(days);
        return Ok(problems);
    }

    [HttpGet("{id:long}")]
    public ActionResult<ProblemDto> GetProblemById(long id)
    {
        try
        {
            var problem = _problemService.GetByIdForAdmin(id);
            return Ok(problem);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("{id:long}/deadline")]
    public ActionResult<ProblemDto> SetDeadline(long id, [FromBody] SetDeadlineDto dto)
    {
        try
        {
            var problem = _problemService.SetAdminDeadline(id, dto.Deadline);
            return Ok(problem);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("check-expired-deadlines")]
    public ActionResult CheckExpiredDeadlines()
    {
        try
        {
            var adminId = User.PersonId();
            _problemService.CheckAndNotifyExpiredDeadlines(adminId);
            return Ok(new { message = "Expired deadlines checked and notifications sent successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:long}/close")]
    public ActionResult<ProblemDto> CloseProblem(long id, [FromBody] CloseProblemDto? dto = null)
    {
        try
        {
            var adminId = User.PersonId();
            var problem = _problemService.CloseProblemAsAdmin(id, adminId, dto?.Comment);
            return Ok(problem);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{id:long}/penalize")]
    public ActionResult PenalizeAuthor(long id)
    {
        try
        {
            var adminId = User.PersonId();
            
            // Get problem to access TourId
            var problem = _problemService.GetByIdForAdmin(id);
            
            // Archive the tour (cross-module operation happens at controller level)
            _tourService.ArchiveTour(problem.TourId);
            
            // Handle problem penalization (notifications, status change)
            _problemService.PenalizeAuthor(id, adminId);
            
            return Ok(new { message = "Author penalized and tour archived successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
