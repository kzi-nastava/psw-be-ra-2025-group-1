using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class TourRatingReaction : Entity
    {
        public long TourRatingId { get; private set; }
        public long UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        // we can add more reaction types in the future

        public TourRatingReaction()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public TourRatingReaction(long tourRatingId, long userId) : this()
        {
            TourRatingId = tourRatingId;
            UserId = userId;
        }
    }
}
