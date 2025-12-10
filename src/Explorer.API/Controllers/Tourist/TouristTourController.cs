using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [AllowAnonymous]
    [Route("api/tourist/tours")]
    [ApiController]
    public class TouristTourController : ControllerBase
    {
        private readonly ITourBrowsingService _tourService;

        public TouristTourController(ITourBrowsingService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        public ActionResult<PagedResult<TourDto>> GetPublished(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            return Ok(_tourService.GetPublished(page, pageSize));
        }

        [HttpGet("{id:long}")]
        public ActionResult<TourDto> GetPublishedById(long id)
        {
            return _tourService.GetPublishedById(id) is { } tour
                ? Ok(tour)
                : NotFound();
        }
    }
}
