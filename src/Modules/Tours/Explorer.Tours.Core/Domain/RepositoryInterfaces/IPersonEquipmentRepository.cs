using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;
    public interface IPersonEquipmentRepository
    {
    PagedResult<PersonEquipment> GetByPersonId(long personId, int page, int pageSize);
    PersonEquipment Add(PersonEquipment personEquipment);
    void Remove(long personId, long equipmentId);
    PersonEquipment? GetByPersonAndEquipment(long personId, long equipmentId);
}

