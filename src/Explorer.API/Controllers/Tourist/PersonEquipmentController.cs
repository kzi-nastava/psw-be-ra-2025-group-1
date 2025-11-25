using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/my-equipments")]
    [ApiController]
    public class PersonEquipmentController : ControllerBase
    {
        private readonly IPersonEquipmentService _personEquipmentService;

        public PersonEquipmentController(IPersonEquipmentService personEquipmentService)
        {
            _personEquipmentService = personEquipmentService;
        }

        [HttpGet]
        public ActionResult<PagedResult<EquipmentDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_personEquipmentService.GetAvailableEquipment(page, pageSize)); //spisak dostupne opreme
        }

        [HttpGet("my-equipments")]
        public ActionResult<PagedResult<PersonEquipmentDto>> GetPersonEquipments([FromQuery] int page, [FromQuery] int pageSize)
        {
            var creatorId = GetPersonId();
            var result = _personEquipmentService.GetPersonEquipment(creatorId, page, pageSize); //paginacija da dobavi opreme od osobe
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<PersonEquipmentDto> Create([FromBody] PersonEquipmentDto personEquipment)
        {
            personEquipment.PersonId = GetPersonId();
            return Ok(_personEquipmentService.AddEquipmentToPerson(personEquipment)); //dodavanje opreme osobi
        }

        [HttpDelete("{eid:long}")]
        public ActionResult Delete(long id, long eid)
        {
            _personEquipmentService.RemoveEquipmentFromPerson(id, eid);
            return Ok();
        }

        private long GetPersonId()
        {
            var personIdClaim = User.FindFirst("personId")?.Value;
            return long.Parse(personIdClaim ?? "0");
        }
    }
}
