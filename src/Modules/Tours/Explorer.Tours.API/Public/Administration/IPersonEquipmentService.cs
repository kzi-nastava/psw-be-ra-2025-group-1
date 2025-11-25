using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;


namespace Explorer.Tours.API.Public;
public interface IPersonEquipmentService
    {
    //PagedResult<PersonEquipmentDto> GetPaged(int page, int pageSize);
    PagedResult<EquipmentDto> GetAvailableEquipment(int page, int pageSize); 
    PagedResult<PersonEquipmentDto> GetPersonEquipment(long personId, int page, int pageSize); //mozda da nazovem GetTouristEquipment, pisem ovaj komentar da ne zaboravim da pitam
    PersonEquipmentDto AddEquipmentToPerson(PersonEquipmentDto personEquipmentDto); //isto
    void RemoveEquipmentFromPerson(long personId, long equipmentId); //isto
}

