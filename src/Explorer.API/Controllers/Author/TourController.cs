using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;


[Authorize(Policy = "authorPolicy")]
[Route("api/author/tours")]
[ApiController]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;
    private readonly ITransportTimeService _transportTimeService;

    public TourController(ITourService tourService, ITransportTimeService transportTimeService)
    {
        _tourService = tourService;
        _transportTimeService = transportTimeService;
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

    [AllowAnonymous]
    [HttpGet("{id:long}/transport-times")]
    public ActionResult<List<TransportTimeDto>> GetTransportTime(long id)
    {
        return Ok(_transportTimeService.GetByTourId(id));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPost("{id:long}/transport-times")]
    public ActionResult<TransportTimeDto> CreateTransportTime([FromBody] TransportTimeDto transport)
    {
        return Ok(_transportTimeService.Create(transport));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpPut("{id:long}/transport-times/{transportId:long}")]
    public ActionResult<TransportTimeDto> UpdateTransportTime([FromBody] TransportTimeDto transport)
    {
        return Ok(_transportTimeService.Update(transport));
    }

    [Authorize(Policy = "authorPolicy")]
    [HttpDelete("{id:long}/transport-times/{transportId:long}")]
    public ActionResult<TransportTimeDto> DeleteTransportTime(long id)
    {
        _transportTimeService.Delete(id);
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
        bool result = _tourService.Publish(id);
        if (result)
        {
            return Ok();
        }
        return BadRequest("Tour could not be published.");
    }

}
