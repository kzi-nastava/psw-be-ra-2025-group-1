using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Route("api/tourist/wallet")]
    [ApiController]
    public class WalletController: ControllerBase
    {
        private readonly IWalletService _service;
        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "touristPolicy")]
        public IActionResult GetWallet()
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var wallet = _service.GetByTouristId(touristId);
            return Ok(wallet);
        }

        [HttpGet("{touristId}")]
        [Authorize(Policy = "administratorPolicy")]
        public IActionResult GetWalletByTouristID(long touristId)
        {
            var wallet = _service.GetByTouristId(touristId);
            return Ok(wallet);
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "administratorPolicy")]
        public IActionResult UpdateBalance(long id, [FromBody] WalletDto request)
        {
            var wallet = _service.UpdateBalance(id, request);
            return Ok(wallet);
        }
    }
}
