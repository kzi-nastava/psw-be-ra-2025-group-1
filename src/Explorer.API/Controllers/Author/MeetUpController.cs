using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/meetup")]
    [ApiController]
    public class MeetUpController : ControllerBase
    {
        private readonly IMeetUpService _meetUpService;

        public MeetUpController(IMeetUpService meetUpService)
        {
            _meetUpService = meetUpService;
        }

        [HttpGet]
        public ActionResult<PagedResult<MeetUpDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_meetUpService.GetPaged(page, pageSize));
        }

        [HttpGet("my")]
        public ActionResult<PagedResult<MeetUpDto>> GetAllByUser([FromQuery] int page, [FromQuery] int pageSize)
        {
            long userId = long.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(_meetUpService.GetPagedByUser(userId, page, pageSize));
        }

        [HttpPost]
        public ActionResult<MeetUpDto> Create([FromBody] MeetUpDto meetUp)
        {
            long userId = long.Parse(User.Claims.First(i => i.Type == "id").Value);
            meetUp.UserId = userId;
            return Ok(_meetUpService.Create(meetUp));
        }

        [HttpPut("{id:long}")]
        public ActionResult<MeetUpDto> Update([FromBody] MeetUpDto meetUp)
        {
            return Ok(_meetUpService.Update(meetUp));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _meetUpService.Delete(id);
            return Ok();
        }
    }
}
