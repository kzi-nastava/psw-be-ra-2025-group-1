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
        public int TourRatingId;
        public int UserId;
        public DateTime CreatedAt;
        // we can add more reaction types in the future

        public TourRatingReaction(int tourRatingId, int userId)
        {
            TourRatingId = tourRatingId;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        private TourRatingReaction() { }
    }
}
