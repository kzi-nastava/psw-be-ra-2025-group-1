using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/problems")]
[ApiController]
public class TouristProblemController : ControllerBase
{
    private readonly IProblemService _problemService;

    public TouristProblemController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    [HttpGet]
    public ActionResult<PagedResult<ProblemDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_problemService.GetPaged(page, pageSize));
    }

    [HttpGet("my-problems")]
    public ActionResult<PagedResult<ProblemDto>> GetMyProblems([FromQuery] int page, [FromQuery] int pageSize)
    {
        var creatorId = User.UserId();
        var result = _problemService.GetByCreator(creatorId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<ProblemDto> GetById(long id)
    {
        try
        {
            var touristId = User.UserId();
            var problem = _problemService.Get(id, touristId);
            return Ok(problem);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost]
    public ActionResult<ProblemDto> Create([FromBody] ProblemDto problem)
    {
        problem.CreatorId = User.UserId();
        problem.CreationTime = DateTime.UtcNow;
        return Ok(_problemService.Create(problem));
    }

    [HttpPut("{id:long}")]
    public ActionResult<ProblemDto> Update([FromBody] ProblemDto problem)
    {
        problem.CreatorId = User.UserId();
        return Ok(_problemService.Update(problem));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _problemService.Delete(id);
        return Ok();
    }

    [HttpPut("{id:long}/status")]
    public ActionResult<ProblemDto> ChangeStatus(long id, [FromBody] ChangeProblemStatusDto dto)
    {
        try
        {
            var touristId = User.UserId();
            var problem = _problemService.ChangeProblemStatus(id, touristId, dto.Status, dto.Comment);
            return Ok(problem);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}