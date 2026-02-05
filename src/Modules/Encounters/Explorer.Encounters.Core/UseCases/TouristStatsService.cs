using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.API.Public;

namespace Explorer.Encounters.Core.UseCases
{
    public class TouristStatsService : ITouristStatsService
    {
        private readonly ITouristStatsRepository _repository;
        private readonly IMapper _mapper;

        public TouristStatsService(ITouristStatsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TouristStatsDto GetByTourist(long touristId)
        {
            return _mapper.Map<TouristStatsDto>(_repository.GetByTourist(touristId));
        }

        public TouristStatsDto Update(TouristStatsDto statsDto)
        {
            var original = _repository.GetByTourist(statsDto.TouristId);

            if (original == null)
                throw new Exception($"TouristStats not found for tourist {statsDto.TouristId}");

            // Map DTO → EXISTING entity
            _mapper.Map(statsDto, original);

            var updatedStats = _repository.Update(original);
            return _mapper.Map<TouristStatsDto>(updatedStats);
        }


        public TouristStatsDto Create(long touristId)
        {
            return _mapper.Map<TouristStatsDto>(_repository.Create(touristId));
        }

        public void AddThumbsUp(long touristId)
        {
            _repository.AddThumbsUp(touristId);
        }

        public void RemoveThumbsUp(long touristId)
        {
            _repository.RemoveThumbsUp(touristId);
        }

        public bool IsLocalGuide(long touristId)
        {
            var stats = _repository.GetByTourist(touristId);
            if (stats == null) return false;
            return stats.IsLocalGuide;
        }

        public void AddRating(long touristId)
        {
            _repository.AddRating(touristId);
        }
        public void RemoveRating(long touristId)
        {
            _repository.RemoveRating(touristId);
        }
    }
}
