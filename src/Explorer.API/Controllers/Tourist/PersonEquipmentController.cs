using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/person-equipment")]
    [ApiController]
    public class PersonEquipmentController : ControllerBase
    {
        private readonly IPersonEquipmentService _personEquipmentService;

        public PersonEquipmentController(IPersonEquipmentService personEquipmentService)
        {
            _personEquipmentService = personEquipmentService;
        }

        [HttpGet("available-equipment")]
        public ActionResult<PagedResult<EquipmentDto>> GetAvailableEquipment([FromQuery] int page, [FromQuery] int pageSize)
        {
            var personId = GetPersonId();
            return Ok(_personEquipmentService.GetAvailableEquipment(personId, page, pageSize));
        }

        [HttpGet("person/{personId:long}")]
        public ActionResult<PagedResult<PersonEquipmentDto>> GetPersonEquipments(long personId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            // Verify the user is requesting their own equipment
            var userPersonId = GetPersonId();
            if (userPersonId != personId)
            {
                return Forbid();
            }
            
            var result = _personEquipmentService.GetPersonEquipment(personId, page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<PersonEquipmentDto> Create([FromBody] PersonEquipmentDto personEquipment)
        {
            personEquipment.PersonId = GetPersonId();
            return Ok(_personEquipmentService.AddEquipmentToPerson(personEquipment));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var personId = GetPersonId();
            _personEquipmentService.RemoveEquipmentFromPerson(personId, id);
            return Ok();
        }

        private long GetPersonId()
        {
            var personIdClaim = User.FindFirst("personId")?.Value;
            return long.Parse(personIdClaim ?? "0");
        }
    }
}
