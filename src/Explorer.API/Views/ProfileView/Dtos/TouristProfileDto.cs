using Explorer.Blog.API.Dtos;
using Explorer.Tours.API.Dtos;

namespace Explorer.API.Views.ProfileView.Dtos
{
    public class TouristProfileDto
    {
        public string Role { get; private set; } = "Tourist";

        public long UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Biography { get; set; }
        public string? Quote { get; set; }

        public int TotalXp { get; set; }
        public int Level { get; set; }

        public List<MapMarkerDto>? MapMarkers { get; set; }
        public MapMarkerDto? ActiveMapMarker { get; set; }

        public List<BlogDto> Blogs { get; set; }
    }
}
