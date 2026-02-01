using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[ApiController]
[Route("api/tours/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherForecastService _weather;

    public WeatherController(IWeatherForecastService weather)
    {
        _weather = weather;
    }

    [HttpGet]
    public async Task<ActionResult<WeatherForecastResponseDto>> Get(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] int hours = 6,
        CancellationToken ct = default)
    {
        if (hours < 1 || hours > 24) return BadRequest("hours must be between 1 and 24.");

        var r = await _weather.GetNextHours(lat, lon, hours, ct);

        var dto = new WeatherForecastResponseDto
        {
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Timezone = r.Timezone,
            Hours = r.Hours.Select(x => new WeatherHourDto
            {
                Time = x.Time,
                TemperatureC = x.TemperatureC,
                PrecipitationMm = x.PrecipitationMm,
                WindSpeedKmh = x.WindSpeedKmh,
                WeatherCode = x.WeatherCode
            }).ToList()
        };

        return Ok(dto);
    }
}