using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Restaurant : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string City { get; private set; }
    public string CuisineType { get; private set; }
    public double AverageRating { get; private set; }      // npr. 4.7
    public int ReviewCount { get; private set; }           // npr. 123
    public string RecommendedDishes { get; private set; }  // tekst: "ćevapi, sarma, gulaš"

    // Za EF
    private Restaurant() { }

    public Restaurant(
        string name,
        string description,
        double latitude,
        double longitude,
        string city,
        string cuisineType,
        double averageRating,
        int reviewCount,
        string recommendedDishes)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? "";
        Latitude = latitude;
        Longitude = longitude;
        City = city ?? "";
        CuisineType = cuisineType ?? "";
        AverageRating = averageRating;
        ReviewCount = reviewCount;
        RecommendedDishes = recommendedDishes ?? "";
    }
}
