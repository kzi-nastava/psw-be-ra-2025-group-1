using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;   
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [ApiController]
    [Route("api/food")]
    public class FoodController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public FoodController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        // GET /api/food/best?lat=...&lng=...&radiusKm=...&maxResults=...
        [HttpGet("best")]
        public ActionResult<List<RestaurantDto>> GetBestRestaurants(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] double? radiusKm,
            [FromQuery] int maxResults = 10)
        {
            // default ako nije poslato ili je glup radius
            var radius = (radiusKm.HasValue && radiusKm.Value > 0) ? radiusKm.Value : 5.0;

            // zaštita da neko ne traži 100000 rezultata
            var safeMaxResults = (maxResults > 0 && maxResults <= 50) ? maxResults : 10;

            var result = _restaurantService.GetBestNearby(lat, lng, radius, safeMaxResults);

            return Ok(result);
        }
    }
}
