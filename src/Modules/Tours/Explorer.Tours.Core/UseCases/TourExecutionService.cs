using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Tours.Core.UseCases;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IUserLocationService _userLocationService;
    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository tourExecutionRepository,
        ITourRepository tourRepository,
        IUserLocationService userLocationService,
        IMapper mapper)
    {
        _tourExecutionRepository = tourExecutionRepository;
        _tourRepository = tourRepository;
        _userLocationService = userLocationService;
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

    public bool CanLeaveReview(long touristId, long tourId)
    {
        var lastExecution = _tourExecutionRepository.GetLastExecutionForTour(touristId, tourId);
        if (lastExecution == null)
            return false;

        return lastExecution.CanLeaveReview();
    }

    public bool CheckIfKeypointReached(long touristId, long executionId)
    {
        var execution = _tourExecutionRepository.Get(executionId);
        var userLocation = _userLocationService.GetByUserId(touristId);

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

        // Get the next keypoint based on CurrentKeypointSequence
        var nextKeypoint = tour.Keypoints
            .FirstOrDefault(k => k.SequenceNumber == execution.CurrentKeypointSequence);

        if (nextKeypoint == null)
            return false; // No more keypoints to reach

        const double nearbyDistance = 0.00018; // approximately 20 meters
        double longDiff = Math.Abs(userLocation.Longitude - nextKeypoint.Longitude);
        double latDiff = Math.Abs(userLocation.Latitude - nextKeypoint.Latitude);

        if (Math.Sqrt(longDiff * longDiff - latDiff * latDiff) < nearbyDistance)
        {
            // Mark keypoint as reached
            if (execution.ReachKeypoint(nextKeypoint.Id, tour.Keypoints.Count))
            {
                _tourExecutionRepository.Update(execution);
                return true;
            }
        }

        return false;
    }
}
