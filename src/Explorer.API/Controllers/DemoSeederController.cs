using Explorer.API.Demo;
using Explorer.Blog.API.Public;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoSeederController : ControllerBase
    {
        private readonly DemoSeeder _demoSeeder;

        public DemoSeederController(
            IAuthenticationService authenticationService, 
            IEquipmentService equipmentService, 
            IFacilityService facilityService, 
            ITourService tourService, 
            IUserLocationService userLocation, 
            ITourExecutionService tourExecution,
            ITourRatingService tourRatingService,
            IRestaurantService restaurantService,
            Explorer.Payments.API.Public.Author.ISaleService saleService,
            IUserManagementService userManagementService,
            IWalletService walletService,
            ITouristMapMarkerService touristMapMarkerService,
            IShoppingCartService shoppingCartService,
            IMapMarkerService mapMarkerService,
            IBlogService blogService)
        {
            _demoSeeder = new DemoSeeder(
                authenticationService, 
                equipmentService, 
                facilityService, 
                tourService, 
                userLocation, 
                tourExecution, 
                tourRatingService, 
                restaurantService,
                saleService,
                userManagementService,
                walletService,
                touristMapMarkerService,
                shoppingCartService,
                mapMarkerService,
                blogService);
        }

        [HttpPost]
        public ActionResult<object> Seed()
        {
            _demoSeeder.Seed();
            return Ok();
        }
    }
}
