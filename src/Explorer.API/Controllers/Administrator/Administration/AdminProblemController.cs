using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/admin/problems")]
[ApiController]
public class AdminProblemController : ControllerBase
{
    private readonly IProblemService _problemService;

    public AdminProblemController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    [HttpGet]
    public ActionResult<PagedResult<ProblemDto>> GetAllProblems([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _problemService.GetPaged(page, pageSize);
        return Ok(result);
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

    [HttpPut("{id:long}/close")]
    public ActionResult<ProblemDto> CloseProblem(long id)
    {
        try
        {
            var adminId = User.PersonId();
            var problem = _problemService.ChangeProblemStatus(id, adminId, ProblemStatus.Unresolved, "Closed by administrator");
            return Ok(problem);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}

public class SetDeadlineDto
{
    public DateTime Deadline { get; set; }
}
