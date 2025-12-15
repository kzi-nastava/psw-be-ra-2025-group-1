using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/problems")]
[ApiController]
public class AuthorProblemController : ControllerBase
{
    private readonly IProblemService _problemService;

    public AuthorProblemController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    [HttpGet]
    public ActionResult<PagedResult<ProblemDto>> GetProblemsForAuthor([FromQuery] int page, [FromQuery] int pageSize)
    {
        var authorId = User.UserId();
        var result = _problemService.GetByAuthor(authorId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<ProblemDto> GetProblemById(long id)
    {
        try
        {
            var authorId = User.UserId();
            var problem = _problemService.Get(id, authorId);
            return Ok(problem);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
