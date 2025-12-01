using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;


[Authorize]
[Route("api/author/tours")]
[ApiController]
public class TourController : BaseController
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
        long authorId = GetCurrentPersonId();
        return Ok(_tourService.Update(id, tour, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{id:long}")]
    public ActionResult<TourDto> Delete(long id)
    {
        long authorId = GetCurrentPersonId();
        _tourService.Delete(id, authorId);
        return Ok();
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{tourId}/keypoints")]
    public ActionResult<KeypointDto> AddKeypoint(long tourId, [FromBody] KeypointDto keypoint)
    {
        long authorId = GetCurrentPersonId();
        return Ok(_tourService.AddKeypoint(tourId, keypoint, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{tourId}/keypoints/{keypointId}")]
    public ActionResult<KeypointDto> UpdateKeypoint(long tourId, long keypointId, [FromBody] KeypointDto keypoint)
    {
        long authorId = GetCurrentPersonId();
        keypoint.Id = keypointId;
        return Ok(_tourService.UpdateKeypoint(tourId, keypoint, authorId));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{tourId}/keypoints/{keypointId}")]
    public ActionResult DeleteKeypoint(long tourId, long keypointId)
    {
        long authorId = GetCurrentPersonId();
        _tourService.DeleteKeypoint(tourId, keypointId, authorId);
        return Ok();
    }
}
