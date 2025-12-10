using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class UserLocationAdapter : IUserLocationRepository
    {
        private readonly IUserLocationService _userLocationService;

        public UserLocationAdapter(IUserLocationService userLocationService)
        {
            _userLocationService = userLocationService;
        }

        public UserLocationDto GetByUserId(long userId)
        {
            return _userLocationService.GetByUserId(userId);
        }
    }
}