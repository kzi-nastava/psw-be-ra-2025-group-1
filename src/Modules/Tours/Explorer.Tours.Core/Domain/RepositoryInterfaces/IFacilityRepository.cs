using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IFacilityRepository
{
    PagedResult<Facility> GetPaged(int page, int pageSize);
    List<Facility> GetAll();
    Facility Get(long id);
    List<Facility> GetByCategory(FacilityCategory category);
    Facility Create(Facility facility);
    Facility Update(Facility facility);
    void Delete(long id);
    bool ExistsByName(string name);
}
