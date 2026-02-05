using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos.Enums;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.Payments.Core.Adapters;

public class TourRepositoryAdapter : ITourRepositoryAdapter
{
    private readonly ITourService _tourService;

    public TourRepositoryAdapter(ITourService tourService)
    {
        _tourService = tourService;
    }

    public bool IsTourPublished(long tourId)
    {
        var tourDto = _tourService.GetById(tourId);
        return tourDto != null && tourDto.Status == TourStatusDto.Published;
    }
}
