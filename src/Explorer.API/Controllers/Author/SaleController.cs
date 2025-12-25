using Explorer.Payments.API.Dtos.Sales;
using Explorer.Payments.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/sales")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost]
    public ActionResult<SaleDto> Create([FromBody] CreateSaleDto createDto)
    {
        long authorId = User.PersonId();
        var result = _saleService.Create(createDto, authorId);
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<SaleDto>> GetAllByAuthor()
    {
        long authorId = User.PersonId();
        var result = _saleService.GetByAuthor(authorId);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<SaleDto> Get(long id)
    {
        var result = _saleService.Get(id);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<SaleDto> Update(long id, [FromBody] UpdateSaleDto updateDto)
    {
        long authorId = User.PersonId();
        var result = _saleService.Update(id, updateDto, authorId);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        long authorId = User.PersonId();
        _saleService.Delete(id, authorId);
        return Ok();
    }
}