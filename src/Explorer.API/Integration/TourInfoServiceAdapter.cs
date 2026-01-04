using Explorer.Payments.Core.Domain.External;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.Payments.Infrastructure.Adapters;

public class TourInfoServiceAdapter : ITourInfoService
{
    private readonly ITourService _tourService;

    public TourInfoServiceAdapter(ITourService tourService)
    {
        _tourService = tourService;
    }

    public TourInfo? GetById(long tourId)
    {
        var t = _tourService.GetById(tourId);
        if (t is null) return null;

        return new TourInfo(
            t.Id,
            t.Title,
            t.Price,
            MapStatus(t.Status.ToString())

        );
    }

    private static TourPublishStatus MapStatus(string status)
        => (status ?? "").Trim().ToLowerInvariant() switch
        {
            "published" => TourPublishStatus.Published,
            "archived"  => TourPublishStatus.Archived,
            _           => TourPublishStatus.Draft
        };
}