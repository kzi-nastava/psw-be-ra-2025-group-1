using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourBrowsingService
{
    PagedResult<TourDto> GetPublished(int page, int pageSize, string? searchTerm = null, string? sortBy = null);
    TourDto? GetPublishedById(long id);
    PagedResult<TourDto> GetToursOnSale(int page, int pageSize, string? sortBy = null);
}
