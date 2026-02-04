using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Dtos.Passkey;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Route("api/passkeys")]
[ApiController]
public class PasskeyController : ControllerBase
{
    private readonly IPasskeyService _passkeyService;

    public PasskeyController(IPasskeyService passkeyService)
    {
        _passkeyService = passkeyService;
    }

    [HttpPost("register/options")]
    [Authorize]
    public ActionResult<PasskeyRegistrationOptionsResponseDto> GetRegistrationOptions([FromBody] PasskeyRegistrationOptionsRequestDto request)
    {
        var userId = long.Parse(User.FindFirst("id")!.Value);
        var username = User.FindFirst("username")!.Value;
        var result = _passkeyService.GetRegistrationOptions(userId, username, request.DeviceName);
        return Ok(result);
    }

    [HttpPost("register/complete")]
    [Authorize]
    public async Task<ActionResult<PasskeyCredentialDto>> CompleteRegistration([FromBody] PasskeyRegistrationCompleteDto request)
    {
        var userId = long.Parse(User.FindFirst("id")!.Value);
        var result = await _passkeyService.CompleteRegistrationAsync(userId, request);
        return Ok(result);
    }

    [HttpPost("login/options")]
    [AllowAnonymous]
    public ActionResult<PasskeyLoginOptionsResponseDto> GetLoginOptions([FromBody] PasskeyLoginOptionsRequestDto request)
    {
        var result = _passkeyService.GetLoginOptions(request.Username);
        return Ok(result);
    }

    [HttpPost("login/complete")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationTokensDto>> CompleteLogin([FromBody] PasskeyLoginCompleteDto request)
    {
        var result = await _passkeyService.CompleteLoginAsync(request);
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public ActionResult<List<PasskeyCredentialDto>> GetUserPasskeys()
    {
        var userId = long.Parse(User.FindFirst("id")!.Value);
        var result = _passkeyService.GetUserPasskeys(userId);
        return Ok(result);
    }

    [HttpPut("{id}/rename")]
    [Authorize]
    public ActionResult<PasskeyCredentialDto> RenamePasskey(long id, [FromBody] PasskeyRenameDto request)
    {
        var userId = long.Parse(User.FindFirst("id")!.Value);
        var result = _passkeyService.RenamePasskey(userId, id, request.DeviceName);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public ActionResult DeletePasskey(long id)
    {
        var userId = long.Parse(User.FindFirst("id")!.Value);
        _passkeyService.DeletePasskey(userId, id);
        return NoContent();
    }
}
