using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/users")]
[ApiController]
public class AuthenticationController : BaseController
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;

    public AuthenticationController(IAuthenticationService authenticationService, IUserRepository userRepository)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
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

    // Debug endpoint to help with troubleshooting - remove in production
    [HttpPost("debug-login")]
    [AllowAnonymous]
    public ActionResult<object> DebugLogin([FromBody] CredentialsDto credentials)
    {
        try
        {
            var result = _authenticationService.Login(credentials);
            return Ok(new {
                success = true,
                token = result.AccessToken,
                userId = result.Id,
                message = "Login successful"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new {
                success = false,
                error = ex.Message,
                message = "Login failed"
            });
        }
    }

    // Debug endpoint to check database connectivity
    [HttpGet("debug-check-user/{username}")]
    [AllowAnonymous]
    public ActionResult<object> DebugCheckUser(string username)
    {
        try
        {
            var userExists = _userRepository.Exists(username);
            var activeUser = _userRepository.GetActiveByName(username);

            return Ok(new {
                username = username,
                exists = userExists,
                isActive = activeUser != null,
                userData = activeUser != null ? new {
                    id = activeUser.Id,
                    role = activeUser.Role.ToString(),
                    isActive = activeUser.IsActive
                } : null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new {
                error = ex.Message,
                message = "Database check failed"
            });
        }
    }
}