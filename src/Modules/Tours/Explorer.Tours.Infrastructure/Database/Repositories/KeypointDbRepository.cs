using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class KeypointDbRepository : IKeypointRepository
{
    private readonly ToursContext _context;

    public KeypointDbRepository(ToursContext context)
    {
        _context = context;
    }

    public List<Keypoint> GetByTourId(long tourId)
    {
        return _context.Keypoints
            .AsNoTracking()
            .Where(k => EF.Property<long>(k, "TourId") == tourId)
            .OrderBy(k => k.SequenceNumber)
            .ToList();
    }
}