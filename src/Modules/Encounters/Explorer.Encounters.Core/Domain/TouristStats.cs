using Explorer.BuildingBlocks.Core.Domain;
using System.Reflection.Metadata;

namespace Explorer.Encounters.Core.Domain
{
    public class TouristStats : Entity
    {
        public long TouristId { get; private set; }
        public int TotalXp { get; private set; }
        public int Level { get; private set; }
        public bool IsLocalGuide { get; set; }
        public int RatingsGiven { get; set; }
        public int ThumbsUpsReceived { get; set; }

        public TouristStats(long touristId)
        {
            TouristId = touristId;
            TotalXp = 0;
            Level = 1;
        }

        private TouristStats() { }

        public void AddXp(int xp)
        {
            if (xp < 0) throw new InvalidOperationException("XP to add cannot be negative");
            TotalXp += xp;
            UpdateLevel();
        }

        private void UpdateLevel()
        {
            if (TotalXp >= Level * 150)
                Level++;
        }

        public void AddRatingGiven()
        {
            RatingsGiven++;
            CheckLocalGuideStatus();
        }

        public void RemoveRatingGiven()
        {
            if (RatingsGiven > 0)
            {
                RatingsGiven--;
                CheckLocalGuideStatus();
            }
        }

        public void AddThumbsUpReceived()
        {
            ThumbsUpsReceived++;
            CheckLocalGuideStatus();
        }

        public void RemoveThumbsUpReceived()
        {
            if (ThumbsUpsReceived > 0)
            {
                ThumbsUpsReceived--;
                CheckLocalGuideStatus();
            }
        }

        private void CheckLocalGuideStatus()
        {
            if (RatingsGiven >= 3 && ThumbsUpsReceived >= 500)
            {
                IsLocalGuide = true;
            }
        }
    }
}
