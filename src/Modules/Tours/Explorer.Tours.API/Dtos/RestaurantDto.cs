namespace Explorer.Tours.API.Dtos;

public class RestaurantDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string City { get; set; } = "";
    public string CuisineType { get; set; } = "";
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string RecommendedDishes { get; set; } = "";
    public double DistanceKm { get; set; }
}
