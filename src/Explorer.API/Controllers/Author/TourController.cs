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
        return Ok(_tourService.Create(tour));
    }

    [AllowAnonymous]
    [HttpGet("{id:long}")]
    public ActionResult<TourDto> Get(long id)
    {
        return _tourService.GetById(id) is { } tour
            ? Ok(tour)
            : NotFound();
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpGet("my")]
    public ActionResult<PagedResult<TourDto>> GetMyToursPaged([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_tourService.GetByCreator(User.PersonId(), page, pageSize));
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
        long authorId = User.PersonId();
        return Ok(_tourService.Update(id, tour, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{id:long}")]
    public ActionResult<TourDto> Delete(long id)
    {
        long authorId = User.PersonId();
        _tourService.Delete(id, authorId);
        return Ok();
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{tourId:long}/transport-times")]
    public ActionResult<TransportTimeDto> AddTransportTime(long tourId, [FromBody] TransportTimeDto transport)
    {
        long authorId = User.PersonId();
        return Ok(_tourService.AddTransportTime(tourId, transport, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{tourId:long}/transport-times/{transportId:long}")]
    public ActionResult<TransportTimeDto> UpdateTransportTime(long tourId,long transportId,[FromBody] TransportTimeDto transport)
    {
        transport.Id = transportId;
        long authorId = User.PersonId();
        return Ok(_tourService.UpdateTransportTime(tourId, transport, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{tourId:long}/transport-times/{transportId:long}")]
    public ActionResult DeleteTransportTime(long tourId, long transportId)
    {
        long authorId = User.PersonId();
        _tourService.DeleteTransportTime(tourId, transportId, authorId);
        return Ok();
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{id:long}/archive")]
    public ActionResult Archive(long id)
    {
        bool result = _tourService.Archive(id);
        if (result)
        {
            return Ok();
        }
        return BadRequest("Tour could not be archived.");
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
                return Ok();
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return BadRequest("Tour could not be published.");

    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{id:long}/activate")]
    public ActionResult Activate(long id)
    {
        bool result = _tourService.Activate(id);
        if (result)
        {
            return Ok();
        }
        return BadRequest("Tour could not be activated.");
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{tourId}/keypoints")]
    public ActionResult<KeypointDto> AddKeypoint(long tourId, [FromBody] KeypointDto keypoint)
    {
        long authorId = User.PersonId();
        return Ok(_tourService.AddKeypoint(tourId, keypoint, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{tourId}/keypoints/{keypointId}")]
    public ActionResult<KeypointDto> UpdateKeypoint(long tourId, long keypointId, [FromBody] KeypointDto keypoint)
    {
        keypoint.Id = keypointId;
        long authorId = User.PersonId();
        return Ok(_tourService.UpdateKeypoint(tourId, keypoint, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{tourId}/keypoints/{keypointId}")]
    public ActionResult DeleteKeypoint(long tourId, long keypointId)
    {
        long authorId = User.PersonId();
        _tourService.DeleteKeypoint(tourId, keypointId, authorId);
        return Ok();
    }
    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{id:long}/equipment/{equipId:long}")]
    public ActionResult<TourDto> AddEquipment(long id, long equipId)
    {
        long authorId = User.PersonId();
        _tourService.AddEquipment(id, equipId, authorId);
        return Ok();
    }
    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{id:long}/equipment/{equipId:long}")]
    public ActionResult<TourDto> RemoveEquipment(long id, long equipId)
    {
        long authorId = User.PersonId();
        _tourService.RemoveEquipment(id, equipId, authorId);
        return Ok();
    }

}
