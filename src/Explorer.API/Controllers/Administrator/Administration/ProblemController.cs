using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/problems")]
[ApiController]
public class AdminProblemController : ControllerBase
{
    private readonly IProblemService _problemService;

    public AdminProblemController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    [HttpGet]
    public ActionResult<PagedResult<ProblemDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_problemService.GetPaged(page, pageSize));
    }

    [HttpGet("by-creator/{creatorId:long}")]
    public ActionResult<PagedResult<ProblemDto>> GetByCreator(long creatorId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_problemService.GetByCreator(creatorId, page, pageSize));
    }

    [HttpPost]
    public ActionResult<ProblemDto> Create([FromBody] ProblemDto problem)
    {
        return Ok(_problemService.Create(problem));
    }

    [HttpPut("{id:long}")]
    public ActionResult<ProblemDto> Update([FromBody] ProblemDto problem)
    {
        return Ok(_problemService.Update(problem));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _problemService.Delete(id);
        return Ok();
    }
}