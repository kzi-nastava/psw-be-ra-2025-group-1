using Explorer.Stakeholders.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.API.Public;

public interface IRatingsService
{
    RatingDto Create(long userId, RatingCreateDto dto);
    List<RatingDto> GetMine(long userId);
    RatingDto Update(long userId, long ratingId, RatingUpdateDto dto);
    void Delete(long userId, long ratingId);
    PagedResult<RatingDto> AdminList(int page, int size);
}
