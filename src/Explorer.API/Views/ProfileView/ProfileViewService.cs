using Explorer.API.Views.ProfileView.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;

namespace Explorer.API.Views.ProfileView
{
    public class ProfileViewService
    {
        private readonly IPersonService _personService;
        private readonly ITouristStatsService _touristStatsService;
        private readonly ITouristMapMarkerService _touristMapMarkerService;
        private readonly IMapMarkerService _mapMarkerService;
        private readonly IUserManagementService _userManagementService;

        public ProfileViewService(
IPersonService personService, ITouristStatsService touristStatsService, ITouristMapMarkerService touristMapMarkerService, IMapMarkerService mapMarkerService, IUserManagementService userManagementService)
        {
            _personService = personService;
            _touristStatsService = touristStatsService;
            _touristMapMarkerService = touristMapMarkerService;
            _mapMarkerService = mapMarkerService;
            _userManagementService = userManagementService;
        }

        public object GetProfileByRole(long userId, string? role)
        {
            if (string.IsNullOrEmpty(role) || role == "Tourist")
                return GetTouristProfile(userId);

            // TODO: Author

            return null;
        }

        private TouristProfileDto GetTouristProfile(long userId)
        {
            var account = _userManagementService.GetById(userId);
            var person = _personService.Get(userId);
            var touristStats = _touristStatsService.GetByTourist(userId);
            var touristMapMarkers = _touristMapMarkerService.GetAllByTourist(userId);
            var mapMarkers = new List<MapMarkerDto>();

            foreach(var touristMapMarker in touristMapMarkers)
            {
                var mapMarker = _mapMarkerService.Get(touristMapMarker.MapMarkerId);
                mapMarkers.Add(mapMarker);
            }

            var activeTouristMapMarker = touristMapMarkers.FirstOrDefault(tmm => tmm.IsActive);

            var activeMapMarker = activeTouristMapMarker != null
                ? _mapMarkerService.Get(activeTouristMapMarker.MapMarkerId)
                : null;

            var TouristProfileDto = new TouristProfileDto()
            {
                UserId = userId,
                Username = account.Username,
                Name = person.Name,
                Surname = person.Surname,
                ProfileImageUrl = person.ProfileImageUrl,
                Biography = person.Biography,
                Quote = person.Quote,
                TotalXp = touristStats.TotalXp,
                Level = touristStats.Level,
                MapMarkers = mapMarkers,
                ActiveMapMarker = activeMapMarker,
            };

            return TouristProfileDto;
        }
    }
}
