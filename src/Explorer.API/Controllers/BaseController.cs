using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");

            return long.Parse(userIdClaim);
        }

        protected long GetCurrentPersonId()
        {
            var personIdClaim = User.FindFirst("personId")?.Value;
            if (personIdClaim == null)
                throw new UnauthorizedAccessException("Person ID not found in token");

            return long.Parse(personIdClaim);
        }
    }
}
