using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using System.Security.Claims;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EncounterController : ControllerBase
{
    private readonly IEncounterService _encounterService;

    public EncounterController(IEncounterService encounterService)
    {
        _encounterService = encounterService;
    }

    [HttpGet("all")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<EncounterDto>> GetAll()
    {
        return Ok(_encounterService.GetAll());
    }

    [HttpGet("active")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<EncounterDto>> GetActive()
    {
        var touristId = User.UserId();
        return Ok(_encounterService.GetAvailableForTourist(touristId));
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<EncounterDto> GetById(long id)
    {
        return Ok(_encounterService.GetById(id));
    }

    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EncounterDto> Create([FromBody] EncounterCreateDto dto)
    {
        return Ok(_encounterService.Create(dto));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EncounterDto> Update(long id, [FromBody] EncounterCreateDto dto)
    {
        return Ok(_encounterService.Update(id, dto));
    }

    [HttpPut("{id}/publish")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Publish(long id)
    {
        _encounterService.Publish(id);
        return Ok();
    }

    [HttpPut("{id}/archive")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Archive(long id)
    {
        _encounterService.Archive(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Delete(long id)
    {
        _encounterService.Delete(id);
        return Ok();
    }

    // Tourist activation and location tracking endpoints
    [HttpPost("{id}/activate")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<ActiveEncounterDto> Activate(long id, [FromBody] LocationDto location)
    {
        var touristId = User.UserId();
        var result = _encounterService.ActivateEncounter(id, touristId, location.Latitude, location.Longitude);
        return Ok(result);
    }

    [HttpPost("location-update")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<ActiveEncounterDto>> UpdateLocation([FromBody] LocationDto location)
    {
        var touristId = User.UserId();
        var result = _encounterService.UpdateTouristLocation(touristId, location.Latitude, location.Longitude);
        return Ok(result);
    }

    [HttpGet("my-active")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<ActiveEncounterDto>> GetMyActiveEncounters()
    {
        var touristId = User.UserId();
        return Ok(_encounterService.GetActiveTouristEncounters(touristId));
    }

    [HttpGet("{id}/active-count")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<int> GetActiveCount(long id)
    {
        return Ok(_encounterService.GetActiveCountInRange(id));
    }

    [HttpGet("{id}/requirements")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<List<RequirementDto>> GetRequirements(long id)
    {
        return Ok(_encounterService.GetRequirementsByActiveEncounter(id));
    }

    [HttpPut("{activeId}/requirements/{reqId}/complete")]
    [Authorize(Policy = "touristPolicy")]
    public IActionResult CompleteRequirement(long activeId, long reqId)
    {
        _encounterService.CompleteRequirement(activeId, reqId);
        return Ok();
    }

    [HttpPut("{activeId}/requirements/{reqId}/reset")]
    [Authorize(Policy = "touristPolicy")]
    public IActionResult ResetRequirement(long activeId, long reqId)
    {
        _encounterService.ResetRequirement(activeId, reqId);
        return Ok();
    }

    [HttpGet("{activeId}/hint")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<string>> GetNextHint(long activeId)
    {
        var tourist = User.UserId();
        return Ok(_encounterService.GetNextHint(activeId, tourist));
    }
}
