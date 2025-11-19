using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers
{
    [Route ("api/users/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IPersonService _personService;

        public ProfileController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("{id:long}")]
        [AllowAnonymous]
        public ActionResult<PersonDto> Get(long id)
        {
            return Ok(_personService.Get(id));
        }

        [HttpPut]
        [Authorize(Policy = "authorOrTouristPolicy")]
        public ActionResult<PersonDto> Update([FromBody] PersonDto personDto)
        {
            personDto.UserId = GetCurrentUserId();
            personDto.Id = GetCurrentPersonId();
            return Ok(_personService.Update(personDto));
        }

        // TODO: Make BaseController and have controllers inherit it with basic mehtods such as this
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");

            return long.Parse(userIdClaim);
        }

        private long GetCurrentPersonId()
        {
            var personIdClaim = User.FindFirst("personId")?.Value;
            if (personIdClaim == null)
                throw new UnauthorizedAccessException("Person ID not found in token");

            return long.Parse(personIdClaim);
        }

    }
}
