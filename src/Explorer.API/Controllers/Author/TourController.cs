using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Explorer.API.Controllers.Author;


[Authorize]
[Route("api/author/tours")]
[ApiController]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] CreateTourDto tour)
    {
        try
        {
            var result = _tourService.Create(tour);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to create tours. Author role required." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to create tour", details = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("{id:long}")]
    public ActionResult<TourDto> Get(long id)
    {
        return _tourService.GetById(id) is { } tour
            ? Ok(tour)
            : NotFound(new { message = $"Tour with ID {id} not found" });
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpGet("my")]
    public ActionResult<PagedResult<TourDto>> GetMyToursPaged([FromQuery] int page, [FromQuery] int pageSize)
    {
        try
        {
            var result = _tourService.GetByCreator(User.PersonId(), page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to retrieve your tours", details = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<PagedResult<TourDto>> GetByPage([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_tourService.GetPaged(page, pageSize));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update(long id, [FromBody] TourDto tour)
    {
        try
        {
            long authorId = User.PersonId();
            var result = _tourService.Update(id, tour, authorId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = $"You don't have permission to update this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Tour with ID {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update tour", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        try
        {
            long authorId = User.PersonId();
            _tourService.Delete(id, authorId);
            return Ok(new { message = "Tour successfully deleted" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to delete this tour. Only the tour creator can delete it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Tour with ID {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to delete tour", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{tourId:long}/transport-times")]
    public ActionResult<TransportTimeDto> AddTransportTime(long tourId, [FromBody] TransportTimeDto transport)
    {
        try
        {
            long authorId = User.PersonId();
            var result = _tourService.AddTransportTime(tourId, transport, authorId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to add transport times to this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Tour with ID {tourId} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to add transport time", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{tourId:long}/transport-times/{transportId:long}")]
    public ActionResult<TransportTimeDto> UpdateTransportTime(long tourId, long transportId, [FromBody] TransportTimeDto transport)
    {
        try
        {
            transport.Id = transportId;
            long authorId = User.PersonId();
            var result = _tourService.UpdateTransportTime(tourId, transport, authorId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to update transport times on this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Transport time with ID {transportId} not found on tour {tourId}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update transport time", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{tourId:long}/transport-times/{transportId:long}")]
    public ActionResult DeleteTransportTime(long tourId, long transportId)
    {
        try
        {
            long authorId = User.PersonId();
            _tourService.DeleteTransportTime(tourId, transportId, authorId);
            return Ok(new { message = "Transport time successfully deleted" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to delete transport times from this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Transport time with ID {transportId} not found on tour {tourId}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to delete transport time", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{id:long}/archive")]
    public ActionResult Archive(long id)
    {
        try
        {
            bool result = _tourService.Archive(id);
            if (result)
            {
                return Ok(new { message = "Tour successfully archived" });
            }
            return BadRequest(new { message = "Tour could not be archived. It may already be archived or in an invalid state." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to archive this tour. Only the tour creator can archive it." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to archive tour", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{id:long}/publish")]
    public ActionResult Publish(long id)
    {
        try
        {
            bool result = _tourService.Publish(id);
            if (result)
            {
                return Ok(new { message = "Tour successfully published" });
            }
            return BadRequest(new { message = "Tour could not be published. Ensure all required fields are complete and the tour has at least 2 keypoints adn one transport time. Check that the tour isn't published already." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to publish this tour. Only the tour creator can publish it." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to publish tour", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{id:long}/activate")]
    public ActionResult Activate(long id)
    {
        try
        {
            bool result = _tourService.Activate(id);
            if (result)
            {
                return Ok(new { message = "Tour successfully activated" });
            }
            return BadRequest(new { message = "Tour could not be activated. It must be published before activation." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to activate this tour. Only the tour creator can activate it." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to activate tour", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{tourId}/keypoints")]
    public ActionResult<KeypointDto> AddKeypoint(long tourId, [FromBody] KeypointDto keypoint)
    {
        try
        {
            long authorId = User.PersonId();
            var result = _tourService.AddKeypoint(tourId, keypoint, authorId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to add keypoints to this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Tour with ID {tourId} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to add keypoint", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{tourId}/keypoints/{keypointId}")]
    public ActionResult<KeypointDto> UpdateKeypoint(long tourId, long keypointId, [FromBody] KeypointDto keypoint)
    {
        try
        {
            keypoint.Id = keypointId;
            long authorId = User.PersonId();
            var result = _tourService.UpdateKeypoint(tourId, keypoint, authorId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to update keypoints on this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Keypoint with ID {keypointId} not found on tour {tourId}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update keypoint", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{tourId}/keypoints/{keypointId}")]
    public ActionResult DeleteKeypoint(long tourId, long keypointId)
    {
        try
        {
            long authorId = User.PersonId();
            _tourService.DeleteKeypoint(tourId, keypointId, authorId);
            return Ok(new { message = "Keypoint successfully deleted" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to delete keypoints from this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Keypoint with ID {keypointId} not found on tour {tourId}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to delete keypoint", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{id:long}/equipment/{equipId:long}")]
    public ActionResult<TourDto> AddEquipment(long id, long equipId)
    {
        try
        {
            long authorId = User.PersonId();
            _tourService.AddEquipment(id, equipId, authorId);
            return Ok(new { message = "Equipment successfully added to tour" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to add equipment to this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Tour with ID {id} or equipment with ID {equipId} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to add equipment", details = ex.Message });
        }
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{id:long}/equipment/{equipId:long}")]
    public ActionResult<TourDto> RemoveEquipment(long id, long equipId)
    {
        try
        {
            long authorId = User.PersonId();
            _tourService.RemoveEquipment(id, equipId, authorId);
            return Ok(new { message = "Equipment successfully removed from tour" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "You don't have permission to remove equipment from this tour. Only the tour creator can modify it." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Equipment with ID {equipId} not found on tour {id}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to remove equipment", details = ex.Message });
        }
    }

}
