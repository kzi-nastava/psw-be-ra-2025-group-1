using System.Text.Json.Serialization;
using Explorer.Tours.API.Public;

namespace Explorer.Tours.Core.UseCases;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;


public class OpenMeteoWeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public OpenMeteoWeatherForecastService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<WeatherForecastResult> GetNextHours(double latitude, double longitude, int hours, CancellationToken ct = default)
    {
        hours = Math.Clamp(hours, 1, 24);

        var cacheKey = $"openmeteo:{latitude:F5}:{longitude:F5}:h{hours}";
        if (_cache.TryGetValue(cacheKey, out WeatherForecastResult cached)) return cached;

        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        // forecast_hours ogranicava koliko sati unapred vrati (baš ono što ti treba)
        var url =
            "https://api.open-meteo.com/v1/forecast" +
            $"?latitude={lat}&longitude={lon}" +
            "&hourly=temperature_2m,precipitation,weathercode,windspeed_10m" +
            $"&forecast_hours={hours}" +
            "&timezone=auto";

        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(ct);
        var data = JsonSerializer.Deserialize<OpenMeteoResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Invalid Open-Meteo response.");

        var result = new WeatherForecastResult
        {
            Latitude = data.Latitude,
            Longitude = data.Longitude,
            Timezone = data.Timezone,
            Hours = MergeHourly(data.Hourly)
        };

        // 10 minuta keš – dovoljno da ne spamuješ, a dovoljno sveže pre ture
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        return result;
    }

    private static List<WeatherHour> MergeHourly(OpenMeteoHourly? h)
    {
        if (h?.Time == null) return new List<WeatherHour>();

        var count = h.Time.Length;
        var list = new List<WeatherHour>(count);

        for (var i = 0; i < count; i++)
        {
            list.Add(new WeatherHour
            {
                Time = DateTime.Parse(h.Time[i], CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal),
                TemperatureC = h.Temperature2m != null && i < h.Temperature2m.Length ? h.Temperature2m[i] : null,
                PrecipitationMm = h.Precipitation != null && i < h.Precipitation.Length ? h.Precipitation[i] : null,
                WindSpeedKmh = h.WindSpeed10m != null && i < h.WindSpeed10m.Length ? h.WindSpeed10m[i] : null,
                WeatherCode = h.WeatherCode != null && i < h.WeatherCode.Length ? h.WeatherCode[i] : null,
            });
        }
        return list;
    }

    // Minimalan model za deserializaciju Open-Meteo odgovora
    private sealed class OpenMeteoResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("hourly")]
        public OpenMeteoHourly? Hourly { get; set; }
    }
    
    private sealed class OpenMeteoHourly
    {
        [JsonPropertyName("time")]
        public string[]? Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double[]? Temperature2m { get; set; }

        [JsonPropertyName("precipitation")]
        public double[]? Precipitation { get; set; }

        [JsonPropertyName("weathercode")]
        public int[]? WeatherCode { get; set; }

        [JsonPropertyName("windspeed_10m")]
        public double[]? WindSpeed10m { get; set; }
    }

}
