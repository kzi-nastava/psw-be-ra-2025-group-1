using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
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
        var creatorId = GetPersonId();
        var result = _problemService.GetByCreator(creatorId, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<ProblemDto> Create([FromBody] ProblemDto problem)
    {
        problem.CreatorId = GetPersonId();
        problem.CreationTime = DateTime.UtcNow;
        return Ok(_problemService.Create(problem));
    }

    [HttpPut("{id:long}")]
    public ActionResult<ProblemDto> Update([FromBody] ProblemDto problem)
    {
        problem.CreatorId = GetPersonId();
        return Ok(_problemService.Update(problem));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _problemService.Delete(id);
        return Ok();
    }

    private long GetPersonId()
    {
        var personIdClaim = User.FindFirst("personId")?.Value;
        return long.Parse(personIdClaim ?? "0");
    }
}