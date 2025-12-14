using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseRepository _tourPurchaseRepository;
    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository tourExecutionRepository,
        ITourRepository tourRepository,
        ITourPurchaseRepository tourPurchaseRepository,
        IMapper mapper)
    {
        _tourExecutionRepository = tourExecutionRepository;
        _tourRepository = tourRepository;
        _tourPurchaseRepository = tourPurchaseRepository;
        _mapper = mapper;
    }

    public TourExecutionDto StartTour(long touristId, StartTourDto startTourDto)
    {
        var tour = _tourRepository.Get(startTourDto.TourId);
        if (tour == null)
            throw new NotFoundException($"Tour with ID {startTourDto.TourId} not found");

        if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
            throw new InvalidOperationException("Can only start published or archived tours");

        // Check if tour has been purchased
        if (!_tourPurchaseRepository.HasPurchased(touristId, startTourDto.TourId))
            throw new InvalidOperationException("You must purchase the tour before starting it");

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
}
