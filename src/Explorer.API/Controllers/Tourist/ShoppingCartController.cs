using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Route("api/tourist/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _service;

        public ShoppingCartController(IShoppingCartService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            long touristId = long.Parse(User.FindFirst("personId")!.Value);
            var cart = _service.GetCart(touristId);
            return Ok(cart);
        }

        [HttpPost("add/{tourId}")]
        public IActionResult AddToCart(long tourId)
        {
            long touristId = long.Parse(User.FindFirst("personId")!.Value);
            _service.AddToCart(touristId, tourId);

            var updatedCart = _service.GetCart(touristId);
            return Ok(updatedCart);
        }

        [HttpDelete("remove/{tourId}")]
        public IActionResult RemoveFromCart(long tourId)
        {
            long touristId = long.Parse(User.FindFirst("personId")!.Value);
            _service.RemoveFromCart(touristId, tourId);

            var updatedCart = _service.GetCart(touristId);
            return Ok(updatedCart);
        }

    }
}