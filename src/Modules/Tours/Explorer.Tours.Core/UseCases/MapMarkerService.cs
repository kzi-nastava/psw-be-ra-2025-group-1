using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases
{
    public class MapMarkerService : IMapMarkerService
    {
        private readonly IMapMarkerRepository _mapMarkerRepository;
        private readonly IMapper _mapper;

        public MapMarkerService(IMapper mapper, IMapMarkerRepository mapMarkerRepository)
        {
            _mapMarkerRepository = mapMarkerRepository;
            _mapper = mapper;
        }
    
        public PagedResult<MapMarkerDto> GetPaged(int page, int pageSize)
        {
            var result = _mapMarkerRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<MapMarkerDto>).ToList();
            return new PagedResult<MapMarkerDto>(items, result.TotalCount);
        }

        public List<MapMarkerDto> GetAll()
        {
            var mapMarkers = _mapMarkerRepository.GetAll();
            return mapMarkers.Select(_mapper.Map<MapMarkerDto>).ToList();
        }

        public MapMarkerDto Get(long mapMarkerId)
        {
            var mapMarker = _mapMarkerRepository.Get(mapMarkerId);
            return _mapper.Map<MapMarkerDto>(mapMarker);
        }

        public MapMarkerDto Create(MapMarkerDto mapMarkerDto)
        {
            var mapMarker = new MapMarker(mapMarkerDto.ImagePathUrl, mapMarkerDto.Name);
            var result = _mapMarkerRepository.Create(mapMarker);
            return _mapper.Map<MapMarkerDto>(result);
        }

        public MapMarkerDto Update(MapMarkerDto mapMarkerDto)
        {
            var mapMarker = _mapMarkerRepository.Get(mapMarkerDto.Id);
            mapMarker.Update(mapMarker);
            var result = _mapMarkerRepository.Update(mapMarker);
            return _mapper.Map<MapMarkerDto>(result);
        }

        public void Delete(long id)
        {
            _mapMarkerRepository.Delete(id);
        }
    }
}
