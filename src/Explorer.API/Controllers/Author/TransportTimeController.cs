using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize]
    [Route("api/author/tours/transport")]
    [ApiController]
    public class TransportTimeController: ControllerBase
    {
        private readonly ITransportTimeService _transportTimeService;

        public TransportTimeController(ITransportTimeService transportTimeService)
        {
            _transportTimeService = transportTimeService;
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpPost]
        public ActionResult<TransportTimeDto> Create([FromBody] TransportTimeDto transport)
        {
            return Ok(_transportTimeService.Create(transport));
        }

        [AllowAnonymous]
        [HttpGet("{id:long}")]
        public ActionResult<TransportTimeDto> Get(long id)
        {
            return _transportTimeService.Get(id) is { } transport
                ? Ok(transport)
                : NotFound();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<PagedResult<TransportTimeDto>> GetByPage([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_transportTimeService.GetPaged(page, pageSize));
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpPut("{id:long}")]
        public ActionResult<TransportTimeDto> Update([FromBody] TransportTimeDto transport)
        {
            return Ok(_transportTimeService.Update(transport));
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpDelete("{id:long}")]
        public ActionResult<TransportTimeDto> Delete(long id)
        {
            _transportTimeService.Delete(id);
            return Ok();
        }

    }
}
