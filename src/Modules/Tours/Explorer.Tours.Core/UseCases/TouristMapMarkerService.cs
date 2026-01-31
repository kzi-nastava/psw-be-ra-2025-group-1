using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Dtos.Enums;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases
{
    public class TouristMapMarkerService : ITouristMapMarkerService
    {
        private readonly ITouristMapMarkerRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITourService _tourService;
        private readonly IMapMarkerService _mapMarkerService;

        // Would make much more sense to have a IsDefault
        // property in Marker and just fetch the default marker,
        // but 👍
        private readonly long defaultMapMarkerId = 1;

        public TouristMapMarkerService(ITouristMapMarkerRepository repository, IMapper mapper, ITourService tourService, IMapMarkerService mapMarkerService)
        {
            _repository = repository;
            _mapper = mapper;
            _tourService = tourService;
            _mapMarkerService = mapMarkerService;
        }

        // Not the cleanest solution but 👍
        private void EnsureDefaultMarker(long touristId)
        {
            var markers = _repository.GetAllByTourist(touristId);
            bool hasDefault = markers.Any(m => m.MapMarkerId == defaultMapMarkerId);
            if (!hasDefault)
            {
                var defaultMarker = Collect(touristId, defaultMapMarkerId);
                SetMapMarkerAsActive(touristId, defaultMapMarkerId);
            }
        }

        public PagedResult<TouristMapMarkerDto> GetPagedByTourist(int page, int pageSize, long touristId)
        {
            EnsureDefaultMarker(touristId);
            var result = _repository.GetPagedByTourist(page, pageSize, touristId);
            var items = result.Results.Select(_mapper.Map<TouristMapMarkerDto>).ToList();
            return new PagedResult<TouristMapMarkerDto>(items, result.TotalCount);
        }

        public List<TouristMapMarkerDto> GetAllByTourist(long touristId)
        {
            EnsureDefaultMarker(touristId);

            var markers = _repository.GetAllByTourist(touristId);
            return markers.Select(_mapper.Map<TouristMapMarkerDto>).ToList();
        }

        public TouristMapMarkerDto GetActiveByTourist(long touristId)
        {
            EnsureDefaultMarker(touristId);

            var activeMarker = _repository.GetActiveByTourist(touristId);
            return _mapper.Map<TouristMapMarkerDto>(activeMarker);
        }

        // Don't really need delete, keeping it just in case
        //public void Delete(long id)
        //{
        //    _repository.Delete(id);
        //}

        public TouristMapMarkerDto CollectFromTour(long touristId, long tourId)
        {
            EnsureDefaultMarker(touristId);

            var tour = _tourService.GetById(tourId);
            if(tour == null)
            {
                throw new NotFoundException($"Tour {tourId} not found");
            }
            if(tour.Status == TourStatusDto.Draft)
            {
                throw new InvalidOperationException($"Can't collect marker from a tour in draft");
            }
            // Check if there's an active tour execution and if tourist bought the tour?

            if(tour.MapMarker == null)
            {
                throw new InvalidOperationException($"Tour {tourId} doesn't have a marker");
            }

            var existing = _repository.GetAllByTourist(touristId).FirstOrDefault(tm => tm.MapMarkerId == tour.MapMarker.Id);

            if (existing != null)
            {
                return _mapper.Map<TouristMapMarkerDto>(existing);
            }

            var newMarker = new TouristMapMarker(touristId, tour.MapMarker.Id);
            var created = _repository.Create(newMarker);
            return _mapper.Map<TouristMapMarkerDto>(created);
        }

        public TouristMapMarkerDto Collect(long touristId, long mapMarkerId)
        {
            var existing = _repository.GetAllByTourist(touristId)
                .FirstOrDefault(tm => tm.MapMarkerId == mapMarkerId);

            if (existing != null)
            {
                return _mapper.Map<TouristMapMarkerDto>(existing);
            }

            var mapMarker = _mapMarkerService.Get(mapMarkerId);

            // Don't allow tourist to collect a marker by its id if its not standalone
            if (!mapMarker.IsStandalone)
            {
                throw new InvalidOperationException("User needs to fulfill a requirement to collect marker " + mapMarkerId);
            }

            var newMarker = new TouristMapMarker(touristId, mapMarkerId);
            var created = _repository.Create(newMarker);
            return _mapper.Map<TouristMapMarkerDto>(created);
        }

        public TouristMapMarkerDto SetMapMarkerAsActive(long touristId, long mapMarkerId)
        {
            var markers = _repository.GetAllByTourist(touristId);

            var active = markers.FirstOrDefault(tm => tm.IsActive);
            // If the marker is already active, just return it
            if(active != null && active.MapMarkerId == mapMarkerId)
            {
                return _mapper.Map<TouristMapMarkerDto>(active);
            }
            // Deactivate already active map marker
            if(active != null)
            {
                active.SetActive(false);
                _repository.Update(active);
            }

            // Find marker to activate, throw exc if tourist doesn't own it
            var markerToActivate = markers.FirstOrDefault(tm => tm.MapMarkerId == mapMarkerId) ?? throw new NotFoundException($"Map marker {mapMarkerId} not collected by tourist {touristId}");

            markerToActivate.SetActive(true);

            _repository.Update(markerToActivate);

            return _mapper.Map<TouristMapMarkerDto>(markerToActivate);
        }
    }
}
