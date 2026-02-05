using Explorer.Payments.Core.Domain.External;
using Explorer.Tours.API.Dtos.Enums;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.Payments.Core.Adapters;

public class TourInfoServiceAdapter : ITourInfoService
{
    private readonly ITourService _tourService;

    public TourInfoServiceAdapter(ITourService tourService)
    {
        _tourService = tourService;
    }

    public TourInfo GetById(long tourId)
    {
        var tour = _tourService.GetById(tourId);
        if (tour == null) throw new KeyNotFoundException("Tour does not exist.");

        var status = tour.Status switch
        {
            TourStatusDto.Published => TourPublishStatus.Published,
            TourStatusDto.Archived => TourPublishStatus.Archived,
            _ => TourPublishStatus.Draft
        };

        return new TourInfo(tour.Id, tour.CreatorId, tour.Title, (decimal)tour.Price, status);
    }
}
