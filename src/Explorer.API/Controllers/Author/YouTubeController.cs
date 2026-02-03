// Explorer.API/Controllers/Author/YouTubeController.cs
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/youtube")]
[ApiController]
public class YouTubeController : ControllerBase
{
    private readonly IYouTubeService _youTubeService;

    public YouTubeController(IYouTubeService youTubeService)
    {
        _youTubeService = youTubeService;
    }

    [HttpGet("search-playlists")]
    public async Task<ActionResult<List<YouTubePlaylistDto>>> SearchPlaylists([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            return BadRequest(new { message = "Query must be at least 3 characters long" });

        var playlists = await _youTubeService.SearchPlaylists(query);
        return Ok(playlists);
    }

    [HttpGet("playlists/{playlistId}")]
    public async Task<ActionResult<YouTubePlaylistDto>> GetPlaylistDetails(string playlistId)
    {
        var playlist = await _youTubeService.GetPlaylistDetails(playlistId);
        return Ok(playlist);
    }
}