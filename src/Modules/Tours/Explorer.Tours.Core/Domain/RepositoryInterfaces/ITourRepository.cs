using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    Tour? Get(long id);
    void Delete(long id);
    PagedResult<Tour> GetPaged(int page, int pageSize);
    PagedResult<Tour> GetByCreatorId(long creatorId, int page, int pageSize);
    PagedResult<Tour> GetPublished(int page, int pageSize);
    Tour? GetPublishedById(long id);
    void DeleteMapMarker(MapMarker marker);
}
