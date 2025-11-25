using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserLocationService : IUserLocationService
    {
        private readonly IUserLocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public UserLocationService(IUserLocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public UserLocationDto Create(UserLocationDto userLocation)
        {
            UserLocation? exists = _locationRepository.GetByUserId(userLocation.UserId);

            if (exists == null)
            {
                return _mapper.Map<UserLocationDto>(_locationRepository.Create(_mapper.Map<UserLocation>(userLocation)));
            }
            else
            {
                try
                {
                    exists.Timestamp = DateTime.UtcNow;
                    exists.Longitude = userLocation.Longitude;
                    exists.Latitude = userLocation.Latitude;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return _mapper.Map<UserLocationDto>(_locationRepository.Update(exists));
            }
            }

        public bool Delete(long id)
        {
            return _locationRepository.Delete(id);
        }

        public UserLocationDto Get(long id)
        {
            return _mapper.Map<UserLocationDto>(_locationRepository.Get(id));
        }

        public UserLocationDto GetByUserId(long userId)
        {
            return _mapper.Map<UserLocationDto>(_locationRepository.GetByUserId(userId));
        }

        public UserLocationDto Update(UserLocationDto userLocation)
        {
            var result = _locationRepository.Update(_mapper.Map<UserLocation>(userLocation));
            return _mapper.Map<UserLocationDto>(result);
        }
    }
}
