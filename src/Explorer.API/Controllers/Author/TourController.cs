using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;


[Authorize(Policy = "authorPolicy")]
[Route("api/author/tours")]
[ApiController]
public class TourController : BaseController
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] TourDto tour)
    {
        return Ok(_tourService.Create(tour));
    }

    [HttpGet("{id:long}")]
    public ActionResult<TourDto> Get(long id)
    {
        return _tourService.GetById(id) is { } tour
            ? Ok(tour)
            : NotFound();
    }

    [HttpGet]
    public ActionResult<PagedResult<TourDto>> GetByPage([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_tourService.GetPaged(page, pageSize));
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update(long id, [FromBody] TourDto tour)
    {
        return Ok(_tourService.Update(id, tour));
    }

    [HttpDelete("{id:long}")]
    public ActionResult<TourDto> Delete(long id)
    {
        _tourService.Delete(id);
        return Ok();
    }

}
