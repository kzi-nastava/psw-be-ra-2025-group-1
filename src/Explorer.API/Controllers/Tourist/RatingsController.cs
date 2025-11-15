using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/ratings")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingsService _service;
    public RatingsController(IRatingsService service) => _service = service;

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
        => long.Parse(User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
