using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.Coupons;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
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
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var cart = _service.GetCart(touristId);
            return Ok(cart);
        }

        [HttpPost("add/{tourId}")]
        public IActionResult AddToCart(long tourId)
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            _service.AddToCart(touristId, tourId);
            var updatedCart = _service.GetCart(touristId);
            return Ok(updatedCart);
        }

        [HttpDelete("remove/{tourId}")]
        public IActionResult RemoveFromCart(long tourId)
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            _service.RemoveFromCart(touristId, tourId);
            return Ok(new { message = "Tour removed from cart successfully" });
        }
        
        [HttpPost("coupon")]
        public IActionResult ApplyCoupon([FromBody] ApplyCouponRequestDto req)
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            _service.ApplyCoupon(touristId, req.Code);
            return Ok();
        }

        [HttpDelete("coupon")]
        public IActionResult RemoveCoupon()
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            _service.RemoveCoupon(touristId);
            return Ok();
        }


        [HttpPost("checkout")]
        public ActionResult<List<TourPurchaseTokenDto>> Checkout()
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);

            try
            {
                var tokens = _service.Checkout(touristId);
                return Ok(tokens);
            }
            catch (InvalidOperationException ex)
            {
                // npr. "Only published tours..." / "Tour does not exist" itd.
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}