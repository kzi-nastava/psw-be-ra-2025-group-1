using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourRatingReactionRepository
    {
        PagedResult<TourRatingReaction> GetPaged(int page, int pageSize);
        PagedResult<TourRatingReaction> GetPagedByTourRating(int tourRatingId, int page, int pageSize);
        TourRatingReaction Get(long id);
        TourRatingReaction Create(TourRatingReaction entity);
        TourRatingReaction Update(TourRatingReaction entity);
        void Delete(long id);
    }
}