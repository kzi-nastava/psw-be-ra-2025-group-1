using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface IRestaurantService
{
    /// <summary>
    /// Vraća najbolje ocenjene restorane u zadatom radijusu (km) oko date lokacije.
    /// </summary>
    List<RestaurantDto> GetBestNearby(double latitude, double longitude, double radiusKm, int maxResults);
}
