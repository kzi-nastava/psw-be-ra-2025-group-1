using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
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
        public IActionResult GetWallet()
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var wallet = _service.GetByTouristId(touristId);
            return Ok(wallet);
        }
    }
}
