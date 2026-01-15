using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Social;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public RestaurantService(IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public List<RestaurantDto> GetBestNearby(double latitude, double longitude, double radiusKm, int maxResults)
    {
        var restaurants = _restaurantRepository.GetAll();

        var ranked = restaurants
            .Select(r => new
            {
                Restaurant = r,
                DistanceKm = CalculateDistanceKm(latitude, longitude, r.Latitude, r.Longitude)
            })
            .Where(x => x.DistanceKm <= radiusKm)
            .OrderByDescending(x => x.Restaurant.AverageRating)
            .ThenByDescending(x => x.Restaurant.ReviewCount)
            .ThenBy(x => x.DistanceKm)
            .Take(maxResults)
            .Select(x =>
            {
                var dto = _mapper.Map<RestaurantDto>(x.Restaurant);
                dto.DistanceKm = x.DistanceKm;
                return dto;
            })
            .ToList();

        return ranked;
    }

    // Haversine formula za rastojanje u km
    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // poluprečnik Zemlje u km

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double angle) => Math.PI * angle / 180.0;


    public RestaurantDto Create(RestaurantDto dto)
    {
        var restaurant = new Restaurant(
            dto.Name,
            dto.Description,
            dto.Latitude,
            dto.Longitude,
            dto.City,
            dto.CuisineType,
            dto.AverageRating,
            dto.ReviewCount,
            dto.RecommendedDishes
        );

        var saved = _restaurantRepository.Create(restaurant);
        return _mapper.Map<RestaurantDto>(saved);
    }
}
