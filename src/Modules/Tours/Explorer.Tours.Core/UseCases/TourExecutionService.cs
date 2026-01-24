using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Services;


namespace Explorer.Tours.Core.UseCases;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IUserLocationRepository _userLocationRepository;
    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository tourExecutionRepository,
        ITourRepository tourRepository,
        IUserLocationRepository userLocationService,
        IMapper mapper)
    {
        _tourExecutionRepository = tourExecutionRepository;
        _tourRepository = tourRepository;
        _userLocationRepository = userLocationService;
        _mapper = mapper;
    }

    public TourExecutionDto StartTour(long touristId, StartTourDto startTourDto)
    {
        var tour = _tourRepository.Get(startTourDto.TourId);
        if (tour == null)
            throw new NotFoundException($"Tour with ID {startTourDto.TourId} not found");

        if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
            throw new InvalidOperationException("Can only start published or archived tours");

        var activeTour = _tourExecutionRepository.GetActiveTourByTourist(touristId);
        if (activeTour != null)
            throw new InvalidOperationException("Tourist already has an active tour. Complete or abandon it first.");

        var tourExecution = new TourExecution(touristId, startTourDto.TourId);
        var created = _tourExecutionRepository.Create(tourExecution);

        return _mapper.Map<TourExecutionDto>(created);
    }

    public TourExecutionDto GetActiveTour(long touristId)
    {
        var activeTour = _tourExecutionRepository.GetActiveTourByTourist(touristId);
        if (activeTour == null)
            throw new NotFoundException("No active tour found for this tourist");

        return _mapper.Map<TourExecutionDto>(activeTour);
    }

    public TourExecutionDto CompleteTour(long touristId, long executionId)
    {
        var execution = _tourExecutionRepository.Get(executionId);
        if (execution == null)
            throw new NotFoundException($"Tour execution with ID {executionId} not found");

        if (execution.TouristId != touristId)
            throw new UnauthorizedAccessException("Cannot complete someone else's tour");

        execution.Complete();
        var updated = _tourExecutionRepository.Update(execution);

        return _mapper.Map<TourExecutionDto>(updated);
    }

    public TourExecutionDto AbandonTour(long touristId, long executionId)
    {
        var execution = _tourExecutionRepository.Get(executionId);
        if (execution == null)
            throw new NotFoundException($"Tour execution with ID {executionId} not found");

        if (execution.TouristId != touristId)
            throw new UnauthorizedAccessException("Cannot abandon someone else's tour");

        execution.Abandon();
        var updated = _tourExecutionRepository.Update(execution);

        return _mapper.Map<TourExecutionDto>(updated);
    }

    public List<TourExecutionDto> GetTouristHistory(long touristId)
    {
        var executions = _tourExecutionRepository.GetByTourist(touristId);
        return executions.Select(_mapper.Map<TourExecutionDto>).ToList();
    }

    public bool TryReachKeypoint(long touristId, long executionId, long keypointId)
    {
        var execution = _tourExecutionRepository.Get(executionId);
        var userLocation = _userLocationRepository.GetByUserId(touristId);

        // ---------- Checks ---------
        if (userLocation == null)
            throw new NotFoundException("User location not found. Please update your location first.");

        if (execution == null)
            throw new NotFoundException($"Tour execution with ID {executionId} not found");

        if (execution.TouristId != touristId)
            throw new UnauthorizedAccessException("Cannot check location for someone else's tour");

        if (!execution.IsActive())
            throw new InvalidOperationException("Tour execution is not active");

        var tour = _tourRepository.Get(execution.TourId);
        if (tour == null)
            throw new NotFoundException("Tour not found");

        KeypointDto keypoint = _mapper.Map<KeypointDto>(tour.Keypoints.FirstOrDefault(kp => kp.Id == keypointId));

        if (keypoint == null)
            throw new NotFoundException("Keypoint not found");

        KeypointProgressDto reached = _mapper.Map<KeypointProgressDto>(execution.KeypointProgresses.FirstOrDefault(kp => kp.KeypointId == keypoint.Id));

        if (reached != null)
            return true; // Already reached

        const double nearbyDistance = 0.00025; // approximately 20m
        double longDiff = Math.Abs(userLocation.Longitude - keypoint.Longitude);
        double latDiff = Math.Abs(userLocation.Latitude - keypoint.Latitude);
        if (Math.Sqrt(longDiff * longDiff + latDiff * latDiff) < nearbyDistance)
        {
            // Mark keypoint as reached and create a new Keypoint Progress for it
            execution.ReachKeypoint(keypoint.Id, tour.Keypoints.Count);
            _tourExecutionRepository.Update(execution);
            return true;
        }

        return false;   // Not close enough
    }

    public KeypointDto UnlockKeypoint(long executionId, long keypointId)
    {
        if (executionId == null)
            throw new InvalidOperationException("Execution not found.");

        var execution = _tourExecutionRepository.Get(executionId);
        var tour = _tourRepository.Get(execution.TourId);

        KeypointDto keypoint = _mapper.Map<KeypointDto>(tour.Keypoints.FirstOrDefault(kp => kp.Id == keypointId));

        if (keypoint == null)
            throw new NotFoundException("Keypoint not found");

        KeypointProgress reached = execution.KeypointProgresses.FirstOrDefault(kp => kp.KeypointId == keypoint.Id);

        // Check if there is a KeypointProgress for the reached keypoint
        if (reached == null)
            throw new InvalidOperationException("Keypoint wasn't reached yet!");

        // Check if that keypoint is already unlocked
        if (!reached.IsCompleted())
            reached.MarkCompleted();

        return new KeypointDto
        {
            Id = keypoint.Id,
            SequenceNumber = keypoint.SequenceNumber,
            Title = keypoint.Title,
            Description = keypoint.Description,
            ImageUrl = keypoint.ImageUrl,
            Latitude = keypoint.Latitude,
            Longitude = keypoint.Longitude,
            Secret = keypoint.Secret
        };
    }

    public KeypointViewDto GetNextKeypointInfo(TourExecutionDto executionDto)
    {
        var execution = _mapper.Map<TourExecution>(executionDto);
        var tour = _tourRepository.Get(execution.TourId);
        var nextKeypoint = tour.Keypoints
            .FirstOrDefault(kp => kp.SequenceNumber == execution.CurrentKeypointSequence);
        if (nextKeypoint == null)
            throw new InvalidOperationException("No next keypoint available");

        return new KeypointViewDto
        {
            Id = nextKeypoint.Id,
            SequenceNumber = nextKeypoint.SequenceNumber,
            Title = nextKeypoint.Title,
            Description = nextKeypoint.Description,
            ImageUrl = nextKeypoint.ImageUrl,
            Latitude = nextKeypoint.Latitude,
            Longitude = nextKeypoint.Longitude
        };
    }

    public TourExecutionDto Create(TourExecutionDto tourExecutionDto)
    {
        if (_tourExecutionRepository.Get(tourExecutionDto.Id) != null)
        {
            throw new EntityValidationException("TourExecution with this ID already exists.");
        }
        var execution = new TourExecution
        (
            tourExecutionDto.TouristId,
            tourExecutionDto.TourId
        );
        var result = _tourExecutionRepository.Create(execution);
        return _mapper.Map<TourExecutionDto>(result);
    }
}
