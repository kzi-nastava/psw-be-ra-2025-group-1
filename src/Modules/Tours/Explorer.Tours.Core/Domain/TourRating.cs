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
        public int UserId { get; set; }
        public int TourExecutionId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public int CompletedProcentage { get; set; }
        public int ThumbsUpCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public TourRating(int userId, int tourExecutionId, int stars, string comment, int procentage)
        {
            UserId = userId;
            TourExecutionId = tourExecutionId;
            Stars = stars;
            Comment = comment;
            ThumbsUpCount = 0;
            CompletedProcentage = procentage;
            CreatedAt = DateTime.UtcNow;
        }

        private TourRating() {}

        // Temp, this might change
        public void IncrementThumbsUp()
        {
            ThumbsUpCount++;
        }
    }
}
