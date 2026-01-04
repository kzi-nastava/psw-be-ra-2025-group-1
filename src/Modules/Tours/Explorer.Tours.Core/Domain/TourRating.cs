using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class TourRating : Entity
    {
        public long UserId { get; private set; }
        public long TourExecutionId { get; private set; }
        public int Stars { get; private set; }
        public string Comment { get; private set; }
        public int CompletedProcentage { get; private set; }
        public int ThumbsUpCount { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public TourRating() 
        {
            ThumbsUpCount = 0;
            CompletedProcentage = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public TourRating(long userId, long tourExecutionId, int stars, string comment, int procentage) : this()
        {
            UserId = userId;
            TourExecutionId = tourExecutionId;
            Stars = stars;
            Comment = comment;
            CompletedProcentage = procentage;
        }

        // Temp, this might change
        public void IncrementThumbsUp()
        {
            ThumbsUpCount++;
        }
    }
}
