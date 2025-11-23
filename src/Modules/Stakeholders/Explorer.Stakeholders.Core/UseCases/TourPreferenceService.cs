using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var result = _tourPreferenceRepository.Get(id);
            return _mapper.Map<TourPreferenceDto>(result);
        }

        public TourPreferenceDto GetByPersonId(long personId)
        {
            var result = _tourPreferenceRepository.GetByPersonId(personId);
            return _mapper.Map<TourPreferenceDto>(result);
        }
    }
}
