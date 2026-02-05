using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Route("api/author/local-place")]
[ApiController]
[Authorize(Policy = "authorPolicy")]
public class LocalPlaceController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public LocalPlaceController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [HttpGet]
    public ActionResult<PagedResult<FacilityDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_facilityService.GetPaged(page, pageSize));
    }

    [HttpGet("all")]
    public ActionResult<List<FacilityDto>> GetAllLocalPlaces()
    {
        return Ok(_facilityService.GetAll());
    }

    [HttpGet("{id:long}")]
    public ActionResult<FacilityDto> GetById(long id)
    {
        return Ok(_facilityService.GetById(id));
    }

    [HttpPost]
    public ActionResult<FacilityDto> Create([FromBody] FacilityDto facility)
    {
        var authorId = User.UserId();
        
        // Validate category - authors can only create Restaurant or Store
        if (facility.Category != FacilityCategory.Restaurant && facility.Category != FacilityCategory.Store)
        {
            return BadRequest(new { error = "Authors can only create facilities with category Restaurant or Store." });
        }
        
        // Auto-set IsLocalPlace to true for authors
        facility.IsLocalPlace = true;
        facility.CreatorId = authorId;
        
        return Ok(_facilityService.Create(facility));
    }

    [HttpPut("{id:long}")]
    public ActionResult<FacilityDto> Update(long id, [FromBody] FacilityDto facility)
    {
        var authorId = User.UserId();
        var existingFacility = _facilityService.GetById(id);
        
        // Check if author owns this facility
        if (existingFacility.CreatorId != authorId)
            return Forbid();
        
        // Validate category - authors can only have Restaurant or Store
        if (facility.Category != FacilityCategory.Restaurant && facility.Category != FacilityCategory.Store)
        {
            return BadRequest(new { error = "Authors can only update facilities with category Restaurant or Store." });
        }
        
        facility.Id = id;
        facility.IsLocalPlace = true;
        facility.CreatorId = authorId;
        
        return Ok(_facilityService.Update(facility));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var authorId = User.UserId();
        var existingFacility = _facilityService.GetById(id);
        
        // Check if author owns this facility
        if (existingFacility.CreatorId != authorId)
            return Forbid();
        
        _facilityService.Delete(id);
        return Ok();
    }
}
