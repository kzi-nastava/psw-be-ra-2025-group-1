using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;
public interface IPersonEquipmentService
    {
    PagedResult<EquipmentDto> GetAvailableEquipment(int page, int pageSize); 
    PagedResult<PersonEquipmentDto> GetPersonEquipment(long personId, int page, int pageSize); //mozda da nazovem GetTouristEquipment, pisem ovaj komentar da ne zaboravim da pitam
    PersonEquipmentDto AddEquipmentToPerson(long personId, long equipmentId, int quantity); //isto
    void RemoveEquipmentFromPerson(long personId, long equipmentId); //isto
}

