using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/location")]
    [ApiController]
    public class UserLocationController: ControllerBase
    {
        private readonly IUserLocationService _locationService;

        public UserLocationController(IUserLocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public ActionResult<UserLocationDto> Get()
        {
            long userId = User.PersonId();
            var result = _locationService.GetByUserId(userId);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<UserLocationDto> Create([FromBody] UserLocationDto userLocation)
        {
            long userId = User.PersonId();
            userLocation.UserId = userId;
            return Ok(_locationService.Create(userLocation));
        }

        [HttpPut]
        public ActionResult<UserLocationDto> Update([FromBody] UserLocationDto userLocation)
        {
            long userId = User.PersonId();
            userLocation.UserId = userId;
            return Ok(_locationService.Update(userLocation));
        }

        [HttpDelete]
        public ActionResult Delete(long id)
        {
            return _locationService.Delete(id) ? Ok() : BadRequest();
        }
    }
}
