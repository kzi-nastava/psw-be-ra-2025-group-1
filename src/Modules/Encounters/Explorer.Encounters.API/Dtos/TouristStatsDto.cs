namespace Explorer.Encounters.API.Dtos
{
    public class TouristStatsDto
    {
        public long TouristId { get; set; }
        public int TotalXp { get; set; }
        public int Level { get; set; }
        public bool IsLocalGuide { get; set; }
        public int RatingsGiven { get; set; }
        public int ThumbsUpsReceived { get; set; }
    }
}
