using Explorer.API.Views.ProfileView.Dtos;
using Explorer.Blog.API.Public;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.API.Views.ProfileView
{
    public class ProfileViewService
    {
        private readonly IPersonService _personService;
        private readonly ITouristStatsService _touristStatsService;
        private readonly ITouristMapMarkerService _touristMapMarkerService;
        private readonly IMapMarkerService _mapMarkerService;
        private readonly IUserManagementService _userManagementService;
        private readonly ITourService _tourService;
        private readonly IBlogService _blogService;

        public ProfileViewService(
            IPersonService personService, 
            ITouristStatsService touristStatsService, 
            ITouristMapMarkerService touristMapMarkerService, 
            IMapMarkerService mapMarkerService, 
            IUserManagementService userManagementService,
            ITourService tourService,
            IBlogService blogService)
        {
            _personService = personService;
            _touristStatsService = touristStatsService;
            _touristMapMarkerService = touristMapMarkerService;
            _mapMarkerService = mapMarkerService;
            _userManagementService = userManagementService;
            _tourService = tourService;
            _blogService = blogService;
        }

        public object GetProfileByRole(long userId, AccountRole role)
        {
            var person = _personService.Get(userId) ?? throw new NotFoundException($"Person {userId} not found");

            if (role == null || role == AccountRole.Tourist)
                return GetTouristProfile(userId);

            if(role == AccountRole.Author)
                return GetAuthorProfile(userId);

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

            var blogs = _blogService.GetUserBlogs(userId);

            var TouristProfileDto = new TouristProfileDto()
            {
                UserId = userId,
                Username = account.Username,
                Name = person.Name,
                Surname = person.Surname,
                Email = person.Email,
                ProfileImageUrl = person.ProfileImageUrl,
                Biography = person.Biography,
                Quote = person.Quote,
                TotalXp = touristStats.TotalXp,
                Level = touristStats.Level,
                MapMarkers = mapMarkers,
                ActiveMapMarker = activeMapMarker,
                Blogs = blogs,
            };

            return TouristProfileDto;
        }

        private AuthorProfileDto GetAuthorProfile(long userId)
        {
            var account = _userManagementService.GetById(userId);
            var person = _personService.Get(userId);

            var tours = _tourService.GetAllByCreator(userId);

            var blogs = _blogService.GetUserBlogs(userId);



            var AuthorProfileDto = new AuthorProfileDto()
            {
                UserId = userId,
                Username = account.Username,
                Name = person.Name,
                Surname = person.Surname,
                Email = person.Email,
                ProfileImageUrl = person.ProfileImageUrl,
                Biography = person.Biography,
                Quote = person.Quote,
                Blogs = blogs,
                Tours = tours,
            };

            return AuthorProfileDto;
        }
    }
}
