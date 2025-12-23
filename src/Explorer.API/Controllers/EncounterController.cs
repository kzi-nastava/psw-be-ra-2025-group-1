using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Encounters.API.Dtos;

namespace Explorer.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EncounterController : ControllerBase
{

    [HttpGet("active")]
    [AllowAnonymous]
    public ActionResult<List<EncounterDto>> GetActive()
    {
        return Ok(new List<EncounterDto>());
    }
    [HttpGet("{id}")]
    [AllowAnonymous]
    public ActionResult<EncounterDto> GetById(long id)
    {
        return NotFound("Encounter not found");
    }
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public ActionResult<EncounterDto> Create([FromBody] EncounterCreateDto dto)
    {
        return StatusCode(501, "Not implemented yet");
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public ActionResult<EncounterDto> Update(long id, [FromBody] EncounterCreateDto dto)
    {
        return StatusCode(501, "Not implemented yet");
    }
    [HttpPut("{id}/publish")]
    [Authorize(Roles = "Administrator")]
    public IActionResult Publish(long id)
    {
        return StatusCode(501, "Not implemented yet");
    }

    [HttpPut("{id}/archive")]
    [Authorize(Roles = "Administrator")]
    public IActionResult Archive(long id)
    {
        return StatusCode(501, "Not implemented yet");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public IActionResult Delete(long id)
    {
        return StatusCode(501, "Not implemented yet");
    }
}
