using Explorer.API.Demo;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoSeederController : ControllerBase
    {
        private readonly DemoSeeder _demoSeeder;

        public DemoSeederController(IAuthenticationService authenticationService, IEquipmentService equipmentService, IFacilityService facilityService, ITourService tourService, IUserLocationService userLocation)
        {
            _demoSeeder = new DemoSeeder(authenticationService, equipmentService, facilityService, tourService, userLocation);
        }

        [HttpPost]
        public ActionResult<object> Seed()
        {
            _demoSeeder.Seed();
            return Ok();
        }
    }
}
