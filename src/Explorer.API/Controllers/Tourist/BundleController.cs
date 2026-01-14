using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/bundles")]
[ApiController]
public class BundleController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public BundleController(IBundleService bundleService)
    {
        _bundleService = bundleService;
    }

    [HttpGet]
    public ActionResult<List<BundleDto>> GetAllPublished()
    {
        var result = _bundleService.GetAllPublished();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<BundleDto> Get(long id)
    {
        var result = _bundleService.Get(id);
        return Ok(result);
    }
}
