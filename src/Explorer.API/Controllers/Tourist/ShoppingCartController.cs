using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [AllowAnonymous]
    [Route("api/tourist/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _service;

        public ShoppingCartController(IShoppingCartService service)
        {
            _service = service;
        }

        [Authorize] 
        [HttpPost("add/{tourId}")]
        public IActionResult AddToCart(long tourId)
        {
            long touristId = long.Parse(User.FindFirst("personId")!.Value);

            _service.AddToCart(touristId, tourId);

            return Ok("Added to cart.");
        }

    }
}
