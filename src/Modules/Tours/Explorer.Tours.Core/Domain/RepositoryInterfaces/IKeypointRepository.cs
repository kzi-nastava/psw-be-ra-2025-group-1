namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IKeypointRepository
{
    List<Keypoint> GetByTourId(long tourId);
}