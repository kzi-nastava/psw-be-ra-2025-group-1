using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface IYouTubeService
    {
        Task<List<YouTubePlaylistDto>> SearchPlaylists(string query);
        Task<YouTubePlaylistDto> GetPlaylistDetails(string playlistId);
    }
}
