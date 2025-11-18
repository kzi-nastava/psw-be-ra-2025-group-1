using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "authorOrTouristPolicy")]
    [Route ("api/users/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IPersonService _personService;

        public ProfileController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("{id: long}")]
        public ActionResult<PersonDto> Get(long id)
        {
            return Ok(_personService.Get(id));
        }

        [HttpPut("{id: long}")]
        public ActionResult<PersonDto> UpdateProfile(long id, [FromBody] PersonDto personDto)
        {
            return Ok(_personService.Update(personDto));
        }
    }
}
