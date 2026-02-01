namespace Explorer.Tours.API.Public;

public interface IWeatherForecastService
{
    Task<WeatherForecastResult> GetNextHours(double latitude, double longitude, int hours, CancellationToken ct = default);
}

public class WeatherForecastResult
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Timezone { get; init; }
    public List<WeatherHour> Hours { get; init; } = new();
}

public class WeatherHour
{
    public DateTime Time { get; init; }
    public double? TemperatureC { get; init; }
    public double? PrecipitationMm { get; init; }
    public double? WindSpeedKmh { get; init; }
    public int? WeatherCode { get; init; }
}