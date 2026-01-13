using Explorer.Payments.API.Dtos.Sales;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/sales")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly ISalePublicService _salePublicService;

    public SaleController(ISalePublicService salePublicService)
    {
        _salePublicService = salePublicService;
    }

    [HttpGet("active")]
    public ActionResult<List<SaleDto>> GetActiveSales()
    {
        var result = _salePublicService.GetActiveSales();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<SaleDto> Get(long id)
    {
        try
        {
            var result = _salePublicService.Get(id);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
