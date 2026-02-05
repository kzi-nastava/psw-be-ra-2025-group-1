using Explorer.Payments.API.Dtos.Coupons;
using Explorer.Payments.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/coupons")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponsController(ICouponService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<CouponDto> Create([FromBody] CreateCouponRequestDto req)
        {
            long authorId = long.Parse(User.FindFirst("id")!.Value);
            var created = _service.Create(authorId, req);
            return Ok(created);
        }
    }
}