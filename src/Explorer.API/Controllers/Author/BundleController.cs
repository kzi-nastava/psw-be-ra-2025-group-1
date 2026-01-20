using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/bundles")]
[ApiController]
public class BundleController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public BundleController(IBundleService bundleService)
    {
        _bundleService = bundleService;
    }

    [HttpPost]
    public ActionResult<BundleDto> Create([FromBody] BundleCreationDto bundle)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Create(authorId, bundle);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<BundleDto> Update(long id, [FromBody] BundleCreationDto bundle)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Update(authorId, id, bundle);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var authorId = User.PersonId();
        _bundleService.Delete(authorId, id);
        return NoContent();
    }

    [HttpPut("publish/{id:long}")]
    public ActionResult<BundleDto> Publish(long id)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Publish(authorId, id);
        return Ok(result);
    }

    [HttpPut("archive/{id:long}")]
    public ActionResult<BundleDto> Archive(long id)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Archive(authorId, id);
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<BundleDto>> GetByAuthor()
    {
        var authorId = User.PersonId();
        var result = _bundleService.GetByAuthorId(authorId);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<BundleDto> Get(long id)
    {
        var result = _bundleService.Get(id);
        return Ok(result);
    }
}
