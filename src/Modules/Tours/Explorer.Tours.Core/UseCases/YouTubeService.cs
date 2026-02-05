using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

public class YouTubeService : IYouTubeService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string ApiBaseUrl = "https://www.googleapis.com/youtube/v3";

    public YouTubeService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["YouTube:ApiKey"]
            ?? throw new InvalidOperationException("YouTube API key not configured");
    }

    public async Task<List<YouTubePlaylistDto>> SearchPlaylists(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            return new List<YouTubePlaylistDto>();

        var url = $"{ApiBaseUrl}/search?part=snippet&q={Uri.EscapeDataString(query)}&type=playlist&maxResults=10&key={_apiKey}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var playlists = new List<YouTubePlaylistDto>();

        if (doc.RootElement.TryGetProperty("items", out var items))
        {
            foreach (var item in items.EnumerateArray())
            {
                playlists.Add(new YouTubePlaylistDto
                {
                    Id = item.GetProperty("id").GetProperty("playlistId").GetString()!,
                    Title = item.GetProperty("snippet").GetProperty("title").GetString()!,
                    Description = item.GetProperty("snippet").GetProperty("description").GetString() ?? "",
                    ThumbnailUrl = item.GetProperty("snippet")
                        .GetProperty("thumbnails")
                        .GetProperty("medium")
                        .GetProperty("url").GetString()!
                });
            }
        }

        return playlists;
    }

    public async Task<YouTubePlaylistDto> GetPlaylistDetails(string playlistId)
    {
        var url = $"{ApiBaseUrl}/playlists?part=snippet,contentDetails&id={playlistId}&key={_apiKey}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var item = doc.RootElement.GetProperty("items")[0];

        return new YouTubePlaylistDto
        {
            Id = item.GetProperty("id").GetString()!,
            Title = item.GetProperty("snippet").GetProperty("title").GetString()!,
            Description = item.GetProperty("snippet").GetProperty("description").GetString() ?? "",
            ThumbnailUrl = item.GetProperty("snippet")
                .GetProperty("thumbnails")
                .GetProperty("medium")
                .GetProperty("url").GetString()!,
            ItemCount = item.GetProperty("contentDetails").GetProperty("itemCount").GetInt32()
        };
    }
}