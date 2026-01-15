using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourPurchaseTokenService
{
    List<TourPurchaseTokenDto> GetByUser(long userId);
    bool HasValidToken(long userId, long tourId);
}
