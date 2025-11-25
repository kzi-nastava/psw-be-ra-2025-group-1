using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/preferences")]
    [ApiController]
    public class TourPreferenceController : ControllerBase
    {
        private readonly ITourPreferenceService _tourPreferenceService;

        public TourPreferenceController(ITourPreferenceService tourPreferenceService)
        {
            _tourPreferenceService = tourPreferenceService;
        }


        [HttpGet]
        public ActionResult<TourPreferenceDto> Get()
        {
            long userId = long.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(_tourPreferenceService.GetByUser(userId));
        }

        [HttpPost]
        public ActionResult<TourPreferenceDto> Create([FromBody] TourPreferenceDto tourPreference)
        {
            long userId = long.Parse(User.Claims.First(i => i.Type == "id").Value);
            tourPreference.UserId = userId;
            return Ok(_tourPreferenceService.Create(tourPreference));
        }

        [HttpPut]
        public ActionResult<TourPreferenceDto> Update([FromBody] TourPreferenceDto tourPreference)
        {
            // TODO: Verify that the user is the owner of the preference
            return Ok(_tourPreferenceService.Update(tourPreference));
        }
    }
}
