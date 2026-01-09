using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourRatingService
    {
        TourRatingDto Get (long id);
        PagedResult<TourRatingDto> GetPaged(int page, int pageSize);
        PagedResult<TourRatingDto> GetPagedByUser(long userId, int page, int pageSize);
        PagedResult<TourRatingDto> GetPagedByTourExecution(long tourExecutionId, int page, int pageSize);
        TourRatingDto Create(TourRatingDto entity);
        TourRatingDto Update(long id, TourRatingDto entity);
        void Delete(long id, long userId);
    }
}