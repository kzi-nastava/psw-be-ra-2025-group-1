using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/journal")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IJournalService _service;

    public JournalController(IJournalService service)
    {
        _service = service;
    }

    [HttpGet("mine")]
    [ProducesResponseType(typeof(List<JournalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<List<JournalDto>> GetMine()
    {
        try
        {
            return Ok(_service.GetMine(GetUserId()));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(JournalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<JournalDto> Create([FromBody] JournalCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created = _service.Create(GetUserId(), dto);
            return Ok(created);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(JournalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<JournalDto> GetById(long id)
    {
        try { return Ok(_service.GetById(GetUserId(), id)); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(JournalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<JournalDto> Update(long id, [FromBody] JournalUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var updated = _service.Update(GetUserId(), id, dto);
            return Ok(updated);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        try
        {
            _service.Delete(GetUserId(), id);
            return NoContent();
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }


    [HttpPut("{id:long}/publish")]
    [ProducesResponseType(typeof(JournalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<JournalDto> Publish(long id)
    {
        try
        {
            var published = _service.Publish(GetUserId(), id);
            return Ok(published);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }


    [HttpPost("{id:long}/collaborators")]
    public ActionResult<JournalDto> AddCollaborator(long id, [FromBody] AddCollaboratorRequestDto request)
    {
        try { return Ok(_service.AddCollaborator(GetUserId(), id, request.CollaboratorUsername)); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:long}/collaborators/{collaboratorUserId:long}")]
    public ActionResult<JournalDto> RemoveCollaborator(long id, long collaboratorUserId)
    {
        try { return Ok(_service.RemoveCollaborator(GetUserId(), id, collaboratorUserId)); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }



    private long GetUserId()
        => long.Parse(User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
