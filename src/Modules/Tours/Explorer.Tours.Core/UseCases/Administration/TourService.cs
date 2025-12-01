using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
namespace Explorer.Tours.Core.UseCases.Administration;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITransportTimeRepository _timeRepository;
    private readonly IMapper _mapper;

    public TourService(ITourRepository tourRepository, ITransportTimeRepository timeRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _timeRepository = timeRepository;
        _mapper = mapper;
    }

    public bool Archive(long id)
    {
        var tour = GetById(id);
        bool canArchive = true;
        if (tour.Status == TourStatusDTO.Archived) return true;

        Tour? tourToUpdate = _tourRepository.Get(id);
        if (tourToUpdate != null)
        {
            tourToUpdate.Archive();
            _tourRepository.Update(tourToUpdate);
        }
        return canArchive;
    }

    public TourDto Create(TourDto tourdto)
    {
        var result = _tourRepository.Create(_mapper.Map<Tour>(tourdto));
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id)
    {
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
        return _mapper.Map<TourDto>(_tourRepository.Get(id));
    }

    public PagedResult<TourDto> GetPaged(int page, int pageSize)
    {
        var result = _tourRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }

    public bool Publish(long id)
    {
        var tour = GetById(id);
        bool canPublish = true;

        if (tour.Status == TourStatusDTO.Published) return true;
        if (tour.Title.Length <= 0) canPublish = false;
        if (tour.Description.Length <= 0) canPublish = false;
        if (tour.Difficulty < 1 || tour.Difficulty > 10) canPublish = false;
        if (tour.Tags.Length <= 0) canPublish = false;

        //Additional validation needed for two keypoints or more
        List<TransportTime> transportTimes = _timeRepository.GetByTourId(id).ToList();
        if (transportTimes.Count < 1) canPublish = false;

        if (canPublish)
        {
            Tour? tourToUpdate = _tourRepository.Get(id);
            if (tourToUpdate != null)
            {
                tourToUpdate.Publish();
                _tourRepository.Update(tourToUpdate);
            }
        }
        return canPublish;
    }

    public TourDto Update(long id, TourDto tourDto)
    {
        var tour = _tourRepository.Get(id);
        tour.Update(tourDto.CreatorId, tourDto.Title, tourDto.Description, tourDto.Difficulty,
            tourDto.Tags, (TourStatus)tourDto.Status, tourDto.Price);

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }
}
