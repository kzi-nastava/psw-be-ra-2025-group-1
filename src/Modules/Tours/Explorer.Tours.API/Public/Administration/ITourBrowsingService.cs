using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourBrowsingService
{
    PagedResult<TourDto> GetPublished(int page, int pageSize);
    TourDto? GetPublishedById(long id);
}
