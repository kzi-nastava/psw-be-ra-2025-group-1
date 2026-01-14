using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using System.Security.Claims;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EncounterController : ControllerBase
{
    private readonly IEncounterService _encounterService;
    private readonly ITouristStatsService _statsService;

    public EncounterController(IEncounterService encounterService, ITouristStatsService statsService)
    {
        _encounterService = encounterService;
        _statsService = statsService;
    }

    [HttpGet("all")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<EncounterDto>> GetAll()
    {
        try
        {
            return Ok(_encounterService.GetAll());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("active")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<EncounterDto>> GetActive()
    {
        var touristId = User.UserId();
        try
        {
            return Ok(_encounterService.GetAvailableForTourist(touristId));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<EncounterDto> GetById(long id)
    {
        try
        {
            return Ok(_encounterService.GetById(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EncounterDto> Create([FromBody] EncounterCreateDto dto)
    {
        try
        {
            return Ok(_encounterService.Create(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (EntityValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EncounterDto> Update(long id, [FromBody] EncounterCreateDto dto)
    {
        try
        {
            return Ok(_encounterService.Update(id, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (EntityValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/publish")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Publish(long id)
    {
        try
        {
            _encounterService.Publish(id);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/archive")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Archive(long id)
    {
        try
        {
            _encounterService.Archive(id);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Delete(long id)
    {
        try
        {
            _encounterService.Delete(id);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/activate")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<ActiveEncounterDto> Activate(long id, [FromBody] LocationDto location)
    {
        var touristId = User.UserId();
        try
        {
            var result = _encounterService.ActivateEncounter(id, touristId, location.Latitude, location.Longitude);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("location-update")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<ActiveEncounterDto>> UpdateLocation([FromBody] LocationDto location)
    {
        var touristId = User.UserId();
        try
        {
            var result = _encounterService.UpdateTouristLocation(touristId, location.Latitude, location.Longitude);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("my-active")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<ActiveEncounterDto>> GetMyActiveEncounters()
    {
        var touristId = User.UserId();
        try
        {
            return Ok(_encounterService.GetActiveTouristEncounters(touristId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}/active-count")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<int> GetActiveCount(long id)
    {
        try
        {
            return Ok(_encounterService.GetActiveCountInRange(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}/requirements")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<List<RequirementDto>> GetRequirements(long id)
    {
        try
        {
            return Ok(_encounterService.GetRequirementsByActiveEncounter(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{activeId}/requirements/{reqId}/complete")]
    [Authorize(Policy = "touristPolicy")]
    public IActionResult CompleteRequirement(long activeId, long reqId)
    {
        try
        {
            _encounterService.CompleteRequirement(activeId, reqId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{activeId}/requirements/{reqId}/reset")]
    [Authorize(Policy = "touristPolicy")]
    public IActionResult ResetRequirement(long activeId, long reqId)
    {
        try
        {
            _encounterService.ResetRequirement(activeId, reqId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{activeId}/hint")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<string>> GetNextHint(long activeId)
    {
        var tourist = User.UserId();
        try
        {
            return Ok(_encounterService.GetNextHint(activeId, tourist));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("stats/{touristId}")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<TouristStatsDto> GetTouristStats(long touristId)
    {
        try
        {
            return Ok(_statsService.GetByTourist(touristId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("stats/update/{touristId}")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<TouristStatsDto> UpdateStatsForTourist([FromBody]TouristStatsDto stats, long touristId)
    {
        try
        {
            return Ok(_statsService.Update(stats));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("stats/create/{touristId}")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<TouristStatsDto> CreateStatsForTourist(long touristId)
    {
        try
        {
            return Ok(_statsService.Create(touristId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}