using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/ratings")]
[ApiController]
public class RatingsController : BaseController
{
    private readonly IRatingsService _service;
    public RatingsController(IRatingsService service) => _service = service;
    /*
    [HttpPost]
    public ActionResult<RatingDto> Create([FromBody] RatingCreateDto dto)
        => Ok(_service.Create(GetUserId(), dto));

    [HttpGet("me")]
    public ActionResult<List<RatingDto>> GetMine()
        => Ok(_service.GetMine(GetUserId()));

    [HttpPut("{id:long}")]
    public ActionResult<RatingDto> Update(long id, [FromBody] RatingUpdateDto dto)
        => Ok(_service.Update(GetUserId(), id, dto));

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        _service.Delete(GetUserId(), id);
        return NoContent();
    }

    private long GetUserId()
        => long.Parse(User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);*/


    [HttpPost]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<RatingDto> Create([FromBody] RatingCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = _service.Create(GetUserId(), dto);
            return Ok(created);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(List<RatingDto>), StatusCodes.Status200OK)]
    public ActionResult<List<RatingDto>> GetMine()
    {
        try { return Ok(_service.GetMine(GetUserId())); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<RatingDto> Update(long id, [FromBody] RatingUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var updated = _service.Update(GetUserId(), id, dto);
            return Ok(updated);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        try
        {
            _service.Delete(GetUserId(), id);
            return NoContent();
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    private long GetUserId()
        => long.Parse(User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
