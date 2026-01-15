using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Public
{
    public interface ITouristStatsService
    {
        public TouristStatsDto GetByTourist(long touristId);
        public TouristStatsDto Update(TouristStatsDto statsDto);
        public TouristStatsDto Create(long touristId);
    }
}
