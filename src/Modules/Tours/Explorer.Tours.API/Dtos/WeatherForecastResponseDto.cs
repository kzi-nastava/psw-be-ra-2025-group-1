namespace Explorer.Tours.API.Dtos;

public class WeatherForecastResponseDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Timezone { get; set; }
    public List<WeatherHourDto> Hours { get; set; } = new();
}

public class WeatherHourDto
{
    public DateTime Time { get; set; }                 // local time (timezone=auto)
    public double? TemperatureC { get; set; }
    public double? PrecipitationMm { get; set; }
    public double? WindSpeedKmh { get; set; }
    public int? WeatherCode { get; set; }              // Open-Meteo weathercode
}