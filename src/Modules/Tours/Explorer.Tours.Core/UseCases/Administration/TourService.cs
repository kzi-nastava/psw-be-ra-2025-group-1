using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Dtos.Enums;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Net.Sockets;
namespace Explorer.Tours.Core.UseCases.Administration;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public TourService(ITourRepository tourRepository, IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    public bool Archive(long id)
    {
        var tour = GetById(id);
        if (tour == null) return false;

        if (tour.Status == TourStatusDto.Archived)
        {
            return false;
        }

        Tour? tourToUpdate = _tourRepository.Get(id);
        if (tourToUpdate != null)
        {
            tourToUpdate.Archive();
            _tourRepository.Update(tourToUpdate);
        }
        return true;
    }

    public bool Activate(long id)
    {
        var tour = _tourRepository.Get(id);
        if (tour == null)
        {
            throw new NotFoundException("Tour not found");
        }
        tour.Activate();
        _tourRepository.Update(tour);

        return true;

    }

    public TourDto Create(CreateTourDto createTourDto)
    {
        var result = _tourRepository.Create(_mapper.Map<Tour>(createTourDto));
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id, long authorId)
    {
        var tour = _tourRepository.Get(id);
        if (tour == null)
            throw new KeyNotFoundException($"Tour with id {id} not found.");

        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't delete someone else's tour");
        _tourRepository.Delete(id);
    }

    public List<TourDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public PagedResult<TourDto> GetByCreator(long creatorId, int page, int pageSize)
    {
        var result = _tourRepository.GetByCreatorId(creatorId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }

    public TourDto GetById(long id)
    {
        var tour = _tourRepository.Get(id);
        var dto = _mapper.Map<TourDto>(tour);
        return dto;
    }

    public PagedResult<TourDto> GetPaged(int page, int pageSize)
    {
        var result = _tourRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }

    public bool Publish(long id)
    {
        Tour? tourToUpdate = _tourRepository.Get(id);
        if (tourToUpdate != null)
        {
            if (tourToUpdate.Publish())
            {
                _tourRepository.Update(tourToUpdate);
                return true;
            }
        }
        return false;
    }

    public TourDto Update(long id, TourDto tourDto, long authorId)
    {
        var tour = _tourRepository.Get(id);
        if (tour == null)
        {
            throw new KeyNotFoundException($"Tour with id {id} not found.");
        }

        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't update someone else's tour");
        tour.Update(tourDto.CreatorId, tourDto.Title, tourDto.Description, tourDto.Difficulty,
            tourDto.Tags, (TourStatus)tourDto.Status, tourDto.Price);

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    public void ArchiveTour(long tourId)
    {
        var tour = _tourRepository.Get(tourId);
        tour.Archive();
    }
    public KeypointDto AddKeypoint(long tourId, KeypointDto keypointDto, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't add keypoint to someone else's tour");
        var keypoint = tour.AddKeypoint(_mapper.Map<Keypoint>(keypointDto));
        _tourRepository.Update(tour);

        return _mapper.Map<KeypointDto>(keypoint);
    }

    public KeypointDto UpdateKeypoint(long tourId, KeypointDto keypointDto, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't update keypoint from someone else's tour");
        var keypoint = tour.UpdateKeypoint(_mapper.Map<Keypoint>(keypointDto));
        _tourRepository.Update(tour);

        return _mapper.Map<KeypointDto>(keypoint);
    }

    public void DeleteKeypoint(long tourId, long keypointId, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't delete keypoint from someone else's tour");
        tour.DeleteKeypoint(keypointId);
        var result = _tourRepository.Update(tour);
    }

    public TourDto AddEquipment(long id, long equipmentId, long authorId)
    {
        var tour = _tourRepository.Get(id);
        var equip = _equipmentRepository.Get(equipmentId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't add equipment to someone else's tour");
        tour.AddEquipment(equip);
        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }
    public TourDto RemoveEquipment(long id, long equipmentId, long authorId)
    {
        var tour = _tourRepository.Get(id);
        var equip = _equipmentRepository.Get(equipmentId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't remove equipment from someone else's tour");
        tour.RemoveEquipment(equip);
        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    public TransportTimeDto AddTransportTime(long tourId, TransportTimeDto timeDto, long authorId)
    {
        if (timeDto.Type == TransportTypeDto.Unknown)
            throw new ArgumentException("Transport type cannot be unknown.");

        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't add transport time to someone else's tour");
        var transportTime = tour.AddTransportTime(_mapper.Map<TransportTime>(timeDto));
        _tourRepository.Update(tour);

        return _mapper.Map<TransportTimeDto>(transportTime);
    }
    public TransportTimeDto UpdateTransportTime(long tourId, TransportTimeDto timeDto, long authorId)
    {
        if (timeDto.Type == TransportTypeDto.Unknown)
            throw new ArgumentException("Transport type cannot be unknown.");

        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't update transport time from someone else's tour");
        var tt = tour.UpdateTransportTime(_mapper.Map<TransportTime>(timeDto));
        _tourRepository.Update(tour);

        return _mapper.Map<TransportTimeDto>(tt);
    }

    public void DeleteTransportTime(long tourId, long timeId, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't delete transport time from someone else's tour");
        tour.DeleteTransportTime(timeId);
        var result = _tourRepository.Update(tour);
    }

    public MapMarkerDto AddMapMarker(long tourId, MapMarkerDto mapMarkerDto, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't add map marker to someone else's tour");
        tour.AddMapMarker(_mapper.Map<MapMarker>(mapMarkerDto));
        _tourRepository.Update(tour);

        return _mapper.Map<MapMarkerDto>(mapMarkerDto);
    }

    public MapMarkerDto UpdateMapMarker(long tourId, MapMarkerDto mapMarkerDto, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't update map marker from someone else's tour");
        if(tour.MapMarker == null)
        {
            throw new InvalidOperationException("Tour has no map marker to update");
        }
        mapMarkerDto.Id = tour.MapMarker.Id;
        var mapMarker = tour.UpdateMapMarker(_mapper.Map<MapMarker>(mapMarkerDto));
        _tourRepository.Update(tour);

        return _mapper.Map<MapMarkerDto>(mapMarker);
    }

    public void DeleteMapMarker(long tourId, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        if (tour.CreatorId != authorId)
            throw new InvalidOperationException("Can't delete map marker from someone else's tour");
        var marker = tour.MapMarker;
        tour.DeleteMapMarker();
        _tourRepository.DeleteMapMarker(marker);
        var result = _tourRepository.Update(tour);
    }
}
