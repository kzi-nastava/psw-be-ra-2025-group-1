using Explorer.BuildingBlocks.Core.Services;
using Explorer.Tours.API.Dtos.Enums;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Tours.Core.Services;

public class TourBrowsingAdapter : ITourBrowsingInfo
{
    private readonly ITourBrowsingService _tourBrowsingService;

    public TourBrowsingAdapter(ITourBrowsingService tourBrowsingService)
    {
        _tourBrowsingService = tourBrowsingService;
    }

    public ITourInfo? GetPublishedTourById(long tourId)
    {
        var tour = _tourBrowsingService.GetPublishedById(tourId);
        if (tour == null)
            return null;

        return new TourInfo
        {
            Id = tour.Id,
            Title = tour.Title,
            Price = tour.Price,
            IsPublished = tour.Status == TourStatusDto.Published
        };
    }

    private class TourInfo : ITourInfo
    {
        public long Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public double Price { get; init; }
        public bool IsPublished { get; init; }
    }
}
