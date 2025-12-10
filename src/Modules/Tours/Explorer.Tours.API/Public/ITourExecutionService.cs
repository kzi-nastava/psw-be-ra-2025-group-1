using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface ITourExecutionService
{
    TourExecutionDto StartTour(long touristId, StartTourDto startTourDto);
    TourExecutionDto GetActiveTour(long touristId);
    TourExecutionDto CompleteTour(long touristId, long executionId);
    TourExecutionDto AbandonTour(long touristId, long executionId);
    List<TourExecutionDto> GetTouristHistory(long touristId);
    bool CanLeaveReview(long touristId, long tourId);
}
