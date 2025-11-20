using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/ratings")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingsService _service;
    public RatingsController(IRatingsService service) => _service = service;

    [HttpGet]
    public ActionResult<PagedResult<RatingDto>> AdminList([FromQuery] int page = 1, [FromQuery] int size = 10)
        => Ok(_service.AdminList(page, size));
}
