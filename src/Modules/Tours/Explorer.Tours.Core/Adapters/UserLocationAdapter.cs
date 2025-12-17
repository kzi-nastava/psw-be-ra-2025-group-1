using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Tours.Core.Adapters
{
    public class UserLocationAdapter : Explorer.Tours.Core.Domain.RepositoryInterfaces.IUserLocationRepository
    {
        private readonly IUserLocationService _userLocationService;

        public UserLocationAdapter(IUserLocationService userLocationService)
        {
            _userLocationService = userLocationService;
        }

        public UserLocation GetByUserId(long userId)
        {
            var dto = _userLocationService.GetByUserId(userId);
            return new UserLocation(dto.UserId, dto.Latitude, dto.Longitude, dto.Timestamp);
        }
    }
}