namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface ITouristStatsRepository
    {
        // Tourist stats methods
        TouristStats Create(long touristId);
        TouristStats Update(TouristStats stats);
        TouristStats GetByTourist(long touristId);
        TouristStats GetById(long id);
    }
}
