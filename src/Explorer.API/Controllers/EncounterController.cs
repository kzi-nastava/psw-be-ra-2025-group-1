using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;

namespace Explorer.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EncounterController : ControllerBase
{
    private readonly IEncounterService _encounterService;

    public EncounterController(IEncounterService encounterService)
    {
        _encounterService = encounterService;
    }

    [HttpGet("all")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<EncounterDto>> GetAll()
    {
        return Ok(_encounterService.GetAll());
    }

    [HttpGet("active")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<List<EncounterDto>> GetActive()
    {
        return Ok(_encounterService.GetActiveEncounters());
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "touristOrAdministratorPolicy")]
    public ActionResult<EncounterDto> GetById(long id)
    {
        return Ok(_encounterService.GetById(id));
    }

    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EncounterDto> Create([FromBody] EncounterCreateDto dto)
    {
        return Ok(_encounterService.Create(dto));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EncounterDto> Update(long id, [FromBody] EncounterCreateDto dto)
    {
        return Ok(_encounterService.Update(id, dto));
    }

    [HttpPut("{id}/publish")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Publish(long id)
    {
        _encounterService.Publish(id);
        return Ok();
    }

    [HttpPut("{id}/archive")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Archive(long id)
    {
        _encounterService.Archive(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "administratorPolicy")]
    public IActionResult Delete(long id)
    {
        _encounterService.Delete(id);
        return Ok();
    }
}
