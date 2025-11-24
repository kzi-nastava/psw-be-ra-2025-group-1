using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserLocationService : IUserLocationService
    {
        private readonly IUserLocationRepository _locationRepository ;
        private readonly IMapper _mapper;

        public UserLocationService(IUserLocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public UserLocationDto Create(UserLocationDto userLocation)
        {
            var result = _locationRepository.Create(_mapper.Map<UserLocation>(userLocation));
            return _mapper.Map<UserLocationDto>(result);
        }

        public bool Delete(long id)
        {
            return _locationRepository.Delete(id);
        }

        public List<UserLocationDto> Get(long id)
        {
            List<UserLocationDto> locationList = _mapper.Map<List<UserLocationDto>>(_locationRepository.Get(id));
            return locationList;
        }

        public List<UserLocationDto> GetByUserId(long userId)
        {
           return _mapper.Map<List<UserLocationDto>>(_locationRepository.GetByUserId(userId));
        }

        public UserLocationDto Update(UserLocationDto userLocation)
        {
            var result = _locationRepository.Update(_mapper.Map<UserLocation>(userLocation));
            return _mapper.Map<UserLocationDto>(result);
        }
    }
}
