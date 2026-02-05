using Explorer.API.Views.ProfileView;
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
        private readonly IUserManagementService _userManagementService;
        private readonly ProfileViewService _profileViewService;

        public ProfileController(IPersonService personService, IUserManagementService userManagementService, ProfileViewService profileViewService)
        {
            _personService = personService;
            _userManagementService = userManagementService;
            _profileViewService = profileViewService;
        }

        [HttpGet("{id:long}")]
        [AllowAnonymous]
        public ActionResult<object> Get(long id)
        {
            var role = _userManagementService.GetById(id).Role;

            var dto = _profileViewService.GetProfileByRole(id, role);

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPut]
        [Authorize(Policy = "authorOrTouristPolicy")]
        public ActionResult<PersonDto> Update([FromBody] PersonDto personDto)
        {
            personDto.UserId = GetCurrentUserId();
            //personDto.Id = GetCurrentPersonId();
           var personId = GetCurrentPersonId();
            return Ok(_personService.Update(personId, personDto));
        }

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
