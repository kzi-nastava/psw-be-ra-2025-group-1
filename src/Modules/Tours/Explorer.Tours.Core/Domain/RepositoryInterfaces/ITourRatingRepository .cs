using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourRatingRepository
    {
        PagedResult<TourRating> GetPaged(int page, int pageSize);
        PagedResult<TourRating> GetPagedByUser(long userId, int page, int pageSize);
        PagedResult<TourRating> GetPagedByTourExecution(long tourExecutionId, int page, int pageSize);
        TourRating Get(long id);
        TourRating Create(TourRating entity);
        TourRating Update(TourRating entity);
        void Delete(long id);
    }
}