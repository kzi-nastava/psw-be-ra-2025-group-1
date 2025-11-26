using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/facility")]
[ApiController]
public class TouristFacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public TouristFacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [HttpGet]
    public ActionResult<List<FacilityDto>> GetAll()
    {
        return Ok(_facilityService.GetAll());
    }

    [HttpGet("{id:long}")]
    public ActionResult<FacilityDto> GetById(long id)
    {
        return Ok(_facilityService.GetById(id));
    }

    [HttpGet("category/{category}")]
    public ActionResult<List<FacilityDto>> GetByCategory(FacilityCategory category)
    {
        return Ok(_facilityService.GetByCategory(category));
    }
}
