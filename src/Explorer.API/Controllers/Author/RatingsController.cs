using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/ratings")]
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
    }*/



    [HttpPost]
    public ActionResult<RatingDto> Create([FromBody] RatingCreateDto dto)
    {
        var err = ValidateCreate(dto);
        if (err != null) return BadRequest(err);

        try
        {
            var created = _service.Create(GetUserId(), dto);
            return Ok(created);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // 403
        }
        catch (KeyNotFoundException)
        {
            return NotFound(); // 404 (ako servis tako baci)
        }
    }

    [HttpGet("me")]
    public ActionResult<List<RatingDto>> GetMine()
    {
        // namerno bez try/catch: treba da bude 200 i kada je lista prazna
        var list = _service.GetMine(GetUserId()) ?? new List<RatingDto>();
        return Ok(list);
    }

    [HttpPut("{id:long}")]
    public ActionResult<RatingDto> Update(long id, [FromBody] RatingUpdateDto dto)
    {
        var err = ValidateUpdate(dto);
        if (err != null) return BadRequest(err);

        try
        {
            var updated = _service.Update(GetUserId(), id, dto);
            return Ok(updated);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // 403 – nije vlasnik
        }
        catch (KeyNotFoundException)
        {
            return NotFound(); // 404 – ne postoji
        }
    }

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        try
        {
            _service.Delete(GetUserId(), id);
            return NoContent(); // 204
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private long GetUserId()
       => long.Parse(User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static string? ValidateCreate(RatingCreateDto dto)
    {
        if (dto == null) return "Body is required.";
        if (dto.Score < 1 || dto.Score > 5) return "Score must be 1–5.";
        if (dto.Comment != null && dto.Comment.Length > 500) return "Comment max length is 500.";
        return null;
    }

    private static string? ValidateUpdate(RatingUpdateDto dto)
    {
        if (dto == null) return "Body is required.";
        if (dto.Score < 1 || dto.Score > 5) return "Score must be 1–5.";
        if (dto.Comment != null && dto.Comment.Length > 500) return "Comment max length is 500.";
        return null;
    }
}
