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
            var stats = _mapper.Map<TouristStats>(statsDto);
            var updatedStats = _repository.Update(stats);
            return _mapper.Map<TouristStatsDto>(updatedStats);
        }

        public TouristStatsDto Create(long touristId)
        {
            return _mapper.Map<TouristStatsDto>(_repository.Create(touristId));
        }
    }
}
