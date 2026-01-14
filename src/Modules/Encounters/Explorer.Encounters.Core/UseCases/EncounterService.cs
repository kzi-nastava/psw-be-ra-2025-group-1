using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases;

public class EncounterService : IEncounterService
{
    private readonly IEncounterRepository _repository;
    private readonly IMapper _mapper;

    public EncounterService(IEncounterRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public List<EncounterDto> GetActiveEncounters()
    {
        return _mapper.Map<List<EncounterDto>>(_repository.GetActive());
    }

    public List<EncounterDto> GetAll()
    {
        return _mapper.Map<List<EncounterDto>>(_repository.GetAll());
    }

    public EncounterDto GetById(long id)
    {
        var encounter = _repository.GetById(id);
        return _mapper.Map<EncounterDto>(encounter);
    }

    public EncounterDto Create(EncounterCreateDto dto)
    {
        var encounterType = Enum.Parse<EncounterType>(dto.Type);
        
        var encounter = new Encounter(
            dto.Title,
            dto.Description,
            dto.Longitude,
            dto.Latitude,
            dto.Xp,
            encounterType,
            dto.Requirements,
            dto.RequiredPeopleCount,
            dto.Range
        );

        var created = _repository.Create(encounter);
        return _mapper.Map<EncounterDto>(created);
    }

    public EncounterDto Update(long id, EncounterCreateDto dto)
    {
        var encounter = _repository.GetById(id);
        
        encounter.Update(dto.Title, dto.Description, dto.Longitude, dto.Latitude, dto.Xp, 
            Enum.Parse<EncounterType>(dto.Type), dto.RequiredPeopleCount, dto.Range);
        var updated = _repository.Update(encounter);
        return _mapper.Map<EncounterDto>(updated);
    }

    public void Publish(long id)
    {
        var encounter = _repository.GetById(id);
        encounter.Publish();
        _repository.Update(encounter);
    }

    public void Archive(long id)
    {
        var encounter = _repository.GetById(id);
        encounter.Archive();
        _repository.Update(encounter);
    }

    public void Delete(long id)
    {
        _repository.Delete(id);
    }

    // Tourist activation and location tracking
    public ActiveEncounterDto ActivateEncounter(long encounterId, long touristId, double latitude, double longitude)
    {
        var encounter = _repository.GetById(encounterId);
        
        // Validate encounter is active
        if (encounter.Status != EncounterStatus.Active)
            throw new InvalidOperationException("Encounter must be active to be activated by tourists.");
        
        // Check if already completed
        if (_repository.HasCompletedEncounter(touristId, encounterId))
            throw new InvalidOperationException("Tourist has already completed this encounter.");
        
        // Check if already activated
        var existing = _repository.GetActiveTouristEncounter(touristId, encounterId);
        if (existing != null)
        {
            // Update location instead
            existing.UpdateLocation(latitude, longitude);
            _repository.UpdateActiveEncounter(existing);
            return MapToDto(existing, encounter);
        }
        
        // Calculate if tourist is within range
        var distance = CalculateDistance(latitude, longitude, encounter.Latitude, encounter.Longitude);
        var isWithinRange = encounter.Range.HasValue && distance <= encounter.Range.Value;
        
        if (!isWithinRange && encounter.Type == EncounterType.Social)
            throw new InvalidOperationException($"Tourist is too far from encounter. Distance: {distance:F2}m, Required: {encounter.Range}m");
        
        var activeEncounter = new ActiveEncounter(touristId, encounterId, latitude, longitude);
        var created = _repository.ActivateEncounter(activeEncounter);

        EncounterType type = encounter.Type;
        if (type == EncounterType.Misc)
        {
            if (encounter.Requirements.Count == 0)
                throw new InvalidOperationException("Misc encounter must have at least one requirement.");
            foreach (var requirement in encounter.Requirements)
            {
                var req = new Requirement(requirement);
                _repository.CreateRequirement(req, created.Id);
            }
        }
        
        return MapToDto(created, encounter);
    }

    public List<ActiveEncounterDto> UpdateTouristLocation(long touristId, double latitude, double longitude)
    {
        var activeEncounters = _repository.GetActiveByTourist(touristId);
        var result = new List<ActiveEncounterDto>();
        
        foreach (var activeEncounter in activeEncounters)
        {
            var encounter = _repository.GetById(activeEncounter.EncounterId);
            
            // Calculate if within range
            var distance = CalculateDistance(latitude, longitude, encounter.Latitude, encounter.Longitude);
            var isWithinRange = encounter.Range.HasValue && distance <= encounter.Range.Value;
            
            // Update location
            activeEncounter.UpdateLocation(latitude, longitude);
            
            // Update range flag
            if (isWithinRange != activeEncounter.IsWithinRange)
            {
                if (isWithinRange)
                    activeEncounter.EnterRange();
                else
                    activeEncounter.LeaveRange();
            }
            
            _repository.UpdateActiveEncounter(activeEncounter);
            
            // Check if social encounter is completed
            if (encounter.Type == EncounterType.Social && encounter.RequiredPeopleCount.HasValue)
            {
                var activeInRange = _repository.GetActiveByEncounter(encounter.Id)
                    .Where(ae => ae.IsWithinRange)
                    .ToList();
                
                if (activeInRange.Count >= encounter.RequiredPeopleCount.Value)
                {
                    // Complete encounter for all tourists in range
                    foreach (var ae in activeInRange)
                    {
                        if (!_repository.HasCompletedEncounter(ae.TouristId, encounter.Id))
                        {
                            var completed = new CompletedEncounter(ae.TouristId, encounter.Id, encounter.Xp);
                            _repository.CompleteEncounter(completed);
                        }
                    }
                }
            }
        }
        
        // Return updated active encounters
        return GetActiveTouristEncounters(touristId);
    }

    public List<ActiveEncounterDto> GetActiveTouristEncounters(long touristId)
    {
        var activeEncounters = _repository.GetActiveByTourist(touristId);
        var result = new List<ActiveEncounterDto>();
        
        foreach (var ae in activeEncounters)
        {
            // Filter out completed encounters
            if (_repository.HasCompletedEncounter(touristId, ae.EncounterId))
                continue;
                
            var encounter = _repository.GetById(ae.EncounterId);
            var dto = MapToDto(ae, encounter);
            
            // Calculate current people count in range
            dto.CurrentPeopleInRange = _repository.GetActiveByEncounter(ae.EncounterId)
                .Count(x => x.IsWithinRange);
            
            result.Add(dto);
        }
        
        return result;
    }

    public int GetActiveCountInRange(long encounterId)
    {
        return _repository.GetActiveByEncounter(encounterId)
            .Count(ae => ae.IsWithinRange);
    }

    public List<EncounterDto> GetAvailableForTourist(long touristId)
    {
        var activeEncounters = _repository.GetActive();
        var completedEncounters = _repository.GetCompletedByTourist(touristId);
        var completedIds = completedEncounters.Select(c => c.EncounterId).ToHashSet();
        
        var availableEncounters = activeEncounters
            .Where(e => !completedIds.Contains(e.Id))
            .ToList();
        
        return _mapper.Map<List<EncounterDto>>(availableEncounters);
    }

    public List<RequirementDto> GetRequirementsByActiveEncounter(long activeEncounterId)
    {
        var requirements = _repository.GetRequirementsByActiveEncounter(activeEncounterId);
        return _mapper.Map<List<RequirementDto>>(requirements);
    }

    public void CompleteRequirement(long activeEncounterId, long requirementId)
    {
        var activeEncounter = _repository.GetActiveById(activeEncounterId);
        if (!activeEncounter.IsWithinRange)
            throw new InvalidOperationException("Tourist is not within range of the encounter.");

        var requirement = activeEncounter.GetRequirementById(requirementId);

        var encounter = _repository.GetById(activeEncounter.EncounterId);

        if (requirement.IsMet)
            throw new InvalidOperationException("Requirement is already met.");

        requirement.MarkAsMet();
        _repository.UpdateRequirement(requirement, activeEncounterId);
        
        if (activeEncounter.AreAllRequirementsMet())
        {
            // Complete the encounter for the tourist
            if (!_repository.HasCompletedEncounter(activeEncounter.TouristId, encounter.Id))
            {
                var completed = new CompletedEncounter(activeEncounter.TouristId, encounter.Id, encounter.Xp);
                _repository.CompleteEncounter(completed);
            }
        }
    }

    public void ResetRequirement(long activeEncounterId, long requirementId)
    {
        var activeEncounter = _repository.GetActiveById(activeEncounterId);
        var requirement = activeEncounter.GetRequirementById(requirementId);
        if (!requirement.IsMet)
            throw new InvalidOperationException("Requirement is already not met.");
        requirement.Reset();
        _repository.UpdateRequirement(requirement, activeEncounterId);
    }

    // Haversine formula to calculate distance between two GPS coordinates in meters
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Earth's radius in meters
        
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return R * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private ActiveEncounterDto MapToDto(ActiveEncounter activeEncounter, Encounter encounter)
    {
        return new ActiveEncounterDto
        {
            Id = activeEncounter.Id,
            TouristId = activeEncounter.TouristId,
            EncounterId = activeEncounter.EncounterId,
            ActivationTime = activeEncounter.ActivationTime,
            LastLocationUpdate = activeEncounter.LastLocationUpdate,
            LastLatitude = activeEncounter.LastLatitude,
            LastLongitude = activeEncounter.LastLongitude,
            IsWithinRange = activeEncounter.IsWithinRange,
            EncounterTitle = encounter.Title,
            EncounterDescription = encounter.Description,
            EncounterType = encounter.Type.ToString(),
            RequiredPeopleCount = encounter.RequiredPeopleCount,
            Requirements = _mapper.Map<List<RequirementDto>>(activeEncounter.Requirements)
        };
    }
}
