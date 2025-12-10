using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution Create(TourExecution tourExecution);
    TourExecution Update(TourExecution tourExecution);
    TourExecution? Get(long id);
    TourExecution? GetActiveTourByTourist(long touristId);
    List<TourExecution> GetByTourist(long touristId);
    TourExecution? GetLastExecutionForTour(long touristId, long tourId);
}
