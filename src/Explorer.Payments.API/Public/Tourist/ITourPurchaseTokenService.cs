using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface ITourPurchaseTokenService
    {
        List<TourPurchaseTokenDto> GetByUser(long userId);
        bool HasValidToken(long userId, long tourId);
    }
}