using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult<AuthenticationTokensDto> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        return Ok(_authenticationService.RegisterTourist(account));
    }

    [HttpPost("register-admin")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<AuthenticationTokensDto> RegisterAdmin([FromBody] AccountRegistrationDto account)
    {
        return Ok(_authenticationService.RegisterAdmin(account));
    }

    [HttpPost("register-author")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<AuthenticationTokensDto> RegisterAuthor([FromBody] AccountRegistrationDto account)
    {
        return Ok(_authenticationService.RegisterAuthor(account));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        return Ok(_authenticationService.Login(credentials));
    }
}