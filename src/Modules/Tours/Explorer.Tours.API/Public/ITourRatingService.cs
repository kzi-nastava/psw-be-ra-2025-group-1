using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourRatingService
    {
        PagedResult<TourRatingDto> GetPaged(int page, int pageSize);
        PagedResult<TourRatingDto> GetPagedByUser(int userId, int page, int pageSize);
        PagedResult<TourRatingDto> GetPagedByTourExecution(int tourExecutionId, int page, int pageSize);
        TourRatingDto Create(TourRatingDto entity);
        TourRatingDto Update(TourRatingDto entity);
        void Delete(long id);
    }
}