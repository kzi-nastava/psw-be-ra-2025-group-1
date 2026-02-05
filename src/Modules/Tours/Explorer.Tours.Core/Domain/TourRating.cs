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
        public double CompletedPercentage { get; private set; }
        public int ThumbsUpCount { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public TourRating() 
        {
            ThumbsUpCount = 0;
            CompletedPercentage = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public TourRating(long userId, long tourExecutionId, int stars, string comment, double procentage) : this()
        {
            UserId = userId;
            TourExecutionId = tourExecutionId;
            Stars = stars;
            Comment = comment;
            CompletedPercentage = procentage;
            Validate(); //Returns ArgumentException if num of Stars invalid
        }

        public void IncrementThumbsUp()
        {
            ThumbsUpCount++;
        }
        public void DecrementThumbsUp()
        {
            ThumbsUpCount--;
        }

        public void UpdateRating(string comment, int stars)
        {
            Comment = comment;
            Stars = stars;
            Validate();
        }

        private void Validate()
        {
            if(Stars < 1 || Stars > 5)
            {
                throw new ArgumentException("Stars must be between 1 and 5.");
            }
        }
    }
}
