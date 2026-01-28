using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;

namespace Explorer.Encounters.Infrastructure
{
    public class TouristStatsRepository : ITouristStatsRepository
    {
        private readonly EncounterContext _context;

        public TouristStatsRepository(EncounterContext context)
        {
            _context = context;
        }

        public TouristStats Create(long touristId)
        {
            var stats = new TouristStats(touristId);
            _context.TouristStats.Add(stats);
            _context.SaveChanges();
            return stats;
        }

        public TouristStats Update(TouristStats stats)
        {
            try
            {
                GetById(stats.Id);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Cannot update TouristStats. ID {stats.Id} does not exist.");
            }
            _context.TouristStats.Update(stats);
            _context.SaveChanges();
            return stats;
        }

        public TouristStats GetById(long id)
        {
            return _context.TouristStats.FirstOrDefault(ts => ts.Id == id)
                ?? throw new KeyNotFoundException($"TouristStats with ID {id} not found.");
        }

        public TouristStats GetByTourist(long touristId)
        {
            return _context.TouristStats.FirstOrDefault(ts => ts.TouristId == touristId)
                ?? Create(touristId);
        }

        public TouristStats AddThumbsUp(long touristId)
        {
            TouristStats stats;
            try
            {
                stats = GetByTourist(touristId);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Cannot add ThumbsUp. Tourist ID {touristId} does not exist.");
            }
            stats.AddThumbsUpReceived();
            _context.TouristStats.Update(stats);
            _context.SaveChanges();
            return stats;
        }

        public TouristStats RemoveThumbsUp(long touristId)
        {
            TouristStats stats;
            try
            {
                stats = GetByTourist(touristId);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Cannot remove ThumbsUp. Tourist ID {touristId} does not exist.");
            }
            stats.RemoveThumbsUpReceived();
            _context.TouristStats.Update(stats);
            _context.SaveChanges();
            return stats;
        }

        public TouristStats AddRating(long touristId)
        {
            TouristStats stats;
            try
            {
                stats = GetByTourist(touristId);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Cannot add Rating. Tourist ID {touristId} does not exist.");
            }
            stats.AddRatingGiven();
            _context.TouristStats.Update(stats);
            _context.SaveChanges();
            return stats;
        }

        public TouristStats RemoveRating(long touristId)
        {
            TouristStats stats;
            try
            {
                stats = GetByTourist(touristId);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Cannot remove Rating. Tourist ID {touristId} does not exist.");
            }
            stats.RemoveRatingGiven();
            _context.TouristStats.Update(stats);
            _context.SaveChanges();
            return stats;
        }
    }
}
