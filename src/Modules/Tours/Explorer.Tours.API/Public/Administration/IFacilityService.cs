using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IFacilityService
{
    PagedResult<FacilityDto> GetPaged(int page, int pageSize);
    List<FacilityDto> GetAll();
    FacilityDto GetById(long id);
    List<FacilityDto> GetByCategory(FacilityCategory category);
    FacilityDto Create(FacilityDto facility);
    FacilityDto Update(FacilityDto facility);
    void Delete(long id);
}
