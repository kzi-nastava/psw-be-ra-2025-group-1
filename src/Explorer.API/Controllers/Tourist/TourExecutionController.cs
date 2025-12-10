using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-execution")]
[ApiController]
public class TourExecutionController : ControllerBase
{
    private readonly ITourExecutionService _tourExecutionService;

    public TourExecutionController(ITourExecutionService tourExecutionService)
    {
        _tourExecutionService = tourExecutionService;
    }

    [HttpPost("start")]
    public ActionResult<TourExecutionDto> StartTour([FromBody] StartTourDto startTourDto)
    {
        var touristId = GetTouristId();
        try
        {
            var execution = _tourExecutionService.StartTour(touristId, startTourDto);
            return Ok(execution);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{executionId:long}/check-location")]
    public ActionResult<bool> CheckTouristLocation(long executionId)
    {
        var touristId = GetTouristId();
        try
        {
            bool result = _tourExecutionService.CheckIfKeypointReached(touristId, executionId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("active")]
    public ActionResult<TourExecutionDto> GetActiveTour()
    {
        var touristId = GetTouristId();
        try
        {
            var execution = _tourExecutionService.GetActiveTour(touristId);
            return Ok(execution);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("{executionId:long}/complete")]
    public ActionResult<TourExecutionDto> CompleteTour(long executionId)
    {
        var touristId = GetTouristId();
        try
        {
            var execution = _tourExecutionService.CompleteTour(touristId, executionId);
            return Ok(execution);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{executionId:long}/abandon")]
    public ActionResult<TourExecutionDto> AbandonTour(long executionId)
    {
        var touristId = GetTouristId();
        try
        {
            var execution = _tourExecutionService.AbandonTour(touristId, executionId);
            return Ok(execution);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("history")]
    public ActionResult<List<TourExecutionDto>> GetHistory()
    {
        var touristId = GetTouristId();
        var history = _tourExecutionService.GetTouristHistory(touristId);
        return Ok(history);
    }

    [HttpGet("can-review/{tourId:long}")]
    public ActionResult<bool> CanLeaveReview(long tourId)
    {
        var touristId = GetTouristId();
        var canReview = _tourExecutionService.CanLeaveReview(touristId, tourId);
        return Ok(new { canReview });
    }

    private long GetTouristId()
    {
        var personIdClaim = User.FindFirst("personId")?.Value;
        if (string.IsNullOrEmpty(personIdClaim))
            throw new UnauthorizedAccessException("Tourist ID not found in token");
        
        return long.Parse(personIdClaim);
    }
}
