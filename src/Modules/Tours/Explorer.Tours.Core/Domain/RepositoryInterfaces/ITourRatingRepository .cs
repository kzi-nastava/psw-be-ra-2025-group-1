using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IRatingRepository
    {
        PagedResult<TourRating> GetPaged(int page, int pageSize);
        PagedResult<TourRating> GetPagedByUser(int userId, int page, int pageSize);
        PagedResult<TourRating> GetPagedByTourExecution(int tourExecutionId, int page, int pageSize);
        TourRating Get(long id);
        TourRating Create(TourRating entity);
        TourRating Update(TourRating entity);
        void Delete(long id);
    }
}