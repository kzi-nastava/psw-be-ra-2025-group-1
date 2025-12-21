using System.Linq;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Social
{
    public class RestaurantQueryTests : BaseToursIntegrationTest
    {
        public RestaurantQueryTests(ToursTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public void Gets_best_restaurants_near_given_location()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IRestaurantService>();
            var repo = scope.ServiceProvider.GetRequiredService<IRestaurantRepository>();

            // Ubacimo par restorana u TEST bazu
            repo.Create(new Restaurant(
                name: "Project 72 Wine & Deli",
                description: "Moderan restoran sa lokalnim i internacionalnim jelima i velikim izborom vina.",
                latitude: 45.2551,
                longitude: 19.8450,
                city: "Novi Sad",
                cuisineType: "srpska / internacionalna",
                averageRating: 4.7,
                reviewCount: 320,
                recommendedDishes: "Teleći obrazi; domaći hleb; lokalna vina"
            ));

            repo.Create(new Restaurant(
                name: "Random Food",
                description: "Brza hrana, ništa specijalno.",
                latitude: 45.2552,
                longitude: 19.8452,
                city: "Novi Sad",
                cuisineType: "fast food",
                averageRating: 3.5,
                reviewCount: 20,
                recommendedDishes: "Burger"
            ));

            // ovaj je predaleko – ne bi smeo da upadne u rezultat za mali radius
            repo.Create(new Restaurant(
                name: "Daleko Daleko",
                description: "Restoran jako daleko od centra.",
                latitude: 45.8000,
                longitude: 19.8450,
                city: "Neki Grad",
                cuisineType: "nebitno",
                averageRating: 5.0,
                reviewCount: 1000,
                recommendedDishes: "Sve"
            ));

            var lat = 45.2550;
            var lng = 19.8450;
            var radiusKm = 3.0;
            var maxResults = 10;

            // Act
            var result = service.GetBestNearby(lat, lng, radiusKm, maxResults);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Count <= maxResults);

            // svi rezultati treba da budu u radijusu
            Assert.All(result, r =>
            {
                Assert.True(r.DistanceKm <= radiusKm + 0.1); // mala tolerancija
            });

            // proveri da je kolekcija već sortirana po: rating ↓, reviewCount ↓, distance ↑
            var ordered = result
                .OrderByDescending(r => r.AverageRating)
                .ThenByDescending(r => r.ReviewCount)
                .ThenBy(r => r.DistanceKm)
                .ToList();

            Assert.Equal(ordered.Select(r => r.Id), result.Select(r => r.Id));
        }
    }
}
