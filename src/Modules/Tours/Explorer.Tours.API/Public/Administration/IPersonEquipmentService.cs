using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;


namespace Explorer.Tours.API.Public;
public interface IPersonEquipmentService
    {
    PagedResult<EquipmentDto> GetAvailableEquipment(long personId, int page, int pageSize); 
    PagedResult<PersonEquipmentDto> GetPersonEquipment(long personId, int page, int pageSize);
    PersonEquipmentDto AddEquipmentToPerson(PersonEquipmentDto personEquipmentDto);
    void RemoveEquipmentFromPerson(long personId, long equipmentId);
}

