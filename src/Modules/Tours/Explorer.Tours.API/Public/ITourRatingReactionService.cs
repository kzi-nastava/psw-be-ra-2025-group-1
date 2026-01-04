using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourRatingReactionService
    {
        PagedResult<TourRatingReactionDto> GetPaged(int page, int pageSize);
        PagedResult<TourRatingReactionDto> GetPagedByTourRating(long tourRatingId, int page, int pageSize);
        TourRatingReactionDto Create(TourRatingReactionDto entity);
        TourRatingReactionDto Update(TourRatingReactionDto entity);
        void Delete(long id);
    }
}