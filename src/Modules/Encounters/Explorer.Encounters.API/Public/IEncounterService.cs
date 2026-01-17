using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Public;

public interface IEncounterService
{
    List<EncounterDto> GetActiveEncounters();
    List<EncounterDto> GetAll();
    EncounterDto GetById(long id);
    EncounterDto Create(EncounterCreateDto dto);
    EncounterDto Update(long id, EncounterCreateDto dto);
    void Publish(long id);
    void Archive(long id);
    void Delete(long id);
    
    // Tourist activation and location tracking
    ActiveEncounterDto ActivateEncounter(long encounterId, long touristId, double latitude, double longitude);
    List<ActiveEncounterDto> UpdateTouristLocation(long touristId, double latitude, double longitude);
    List<ActiveEncounterDto> GetActiveTouristEncounters(long touristId);
    int GetActiveCountInRange(long encounterId);
    List<EncounterDto> GetAvailableForTourist(long touristId);

    List<RequirementDto> GetRequirementsByActiveEncounter(long activeEncounterId);
    void CompleteRequirement(long activeEncounterId, long requirementId);
    void ResetRequirement(long activeEncounterId, long requirementId);
    List<string> GetNextHint(long activeId, long touristId);
    public HiddenEncounterDto UpdateHidden(long id, EncounterCreateDto dto);
}