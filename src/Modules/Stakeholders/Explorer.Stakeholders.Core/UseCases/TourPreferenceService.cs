using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TourPreferenceService : ITourPreferenceService
    {
        private readonly ITourPreferenceRepository _tourPreferenceRepository;
        private readonly IMapper _mapper;

        public TourPreferenceService(ITourPreferenceRepository tourPreferenceRepository, IMapper mapper)
        {
            _tourPreferenceRepository = tourPreferenceRepository;
            _mapper = mapper;
        }

        public TourPreferenceDto Create(TourPreferenceDto entity)
        {
            var result = _tourPreferenceRepository.Create(_mapper.Map<TourPreference>(entity));
            return _mapper.Map<TourPreferenceDto>(result);
        }

        public TourPreferenceDto Update(TourPreferenceDto entity)
        {
            var result = _tourPreferenceRepository.Update(_mapper.Map<TourPreference>(entity));
            return _mapper.Map<TourPreferenceDto>(result);
        }

        public TourPreferenceDto Get(long id)
        {
            TourPreference? result = _tourPreferenceRepository.Get(id);

            return result == null ? throw new Exception("TourPreference could not be created") : _mapper.Map<TourPreferenceDto>(result);
        }

        public TourPreferenceDto GetByUser(long userId)
        {
            var result = _tourPreferenceRepository.GetByUser(userId);
            if (result == null)
            {
                TourPreferenceDto tpdto = new();
                tpdto.UserId = userId;
                Create(tpdto);
            }
            result = _tourPreferenceRepository.GetByUser(userId);

            if (result == null)
                throw new Exception("TourPreference could not be created");

            return _mapper.Map<TourPreferenceDto>(result);
        }
    }
}
