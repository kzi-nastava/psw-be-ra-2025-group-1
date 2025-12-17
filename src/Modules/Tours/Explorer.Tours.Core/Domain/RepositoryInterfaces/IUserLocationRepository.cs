using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IUserLocationRepository
    {
        UserLocation GetByUserId(long userId);
    }
}
