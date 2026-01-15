using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/tourist/tour-purchase-tokens")]
    [Authorize(Roles = "tourist, author")]
    public class TourPurchaseTokenController : ControllerBase
    {
        private readonly ITourPurchaseTokenService _tokenService;

        public TourPurchaseTokenController(ITourPurchaseTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // GET api/tour-purchase-tokens/my
        [HttpGet("my")]
        public ActionResult<List<TourPurchaseTokenDto>> GetMyTokens()
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);

            var tokens = _tokenService.GetByUser(touristId);
            return Ok(tokens);
        }

        // GET api/tour-purchase-tokens/has-valid?tourId=123
        [HttpGet("has-valid")]
        public ActionResult<bool> HasValid([FromQuery] long tourId)
        {
            if (tourId <= 0) return BadRequest("tourId must be positive.");

            long touristId = long.Parse(User.FindFirst("id")!.Value);

            var hasValid = _tokenService.HasValidToken(touristId, tourId);
            return Ok(hasValid);
        }
    }
}