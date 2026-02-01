using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface ITourExecutionService
{
    TourExecutionDto StartTour(long touristId, StartTourDto startTourDto);
    TourExecutionDto GetActiveTour(long touristId);
    TourExecutionDto CompleteTour(long touristId, long executionId);
    TourExecutionDto AbandonTour(long touristId, long executionId);
    List<TourExecutionDto> GetTouristHistory(long touristId);
    bool TryReachKeypoint(long touristId, long executionId, long keypointId);
    KeypointDto UnlockKeypoint(long executionId, long keypointId);
    KeypointViewDto GetNextKeypointInfo(TourExecutionDto executionDto);
    TourExecutionDto Create(TourExecutionDto tourExecutionDto);
    List<KeypointDto> GetTourKeypoints(long touristId, long tourId);
}
