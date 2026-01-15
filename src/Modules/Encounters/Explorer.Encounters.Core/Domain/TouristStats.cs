using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class TouristStats : Entity
    {
        public long TouristId { get; private set; }
        public int TotalXp { get; private set; }
        public int Level { get; private set; }

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
    }
}
