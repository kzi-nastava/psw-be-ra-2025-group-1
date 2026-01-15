using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.RepositoryInterfaces;

public interface IEncounterRepository
{
    Encounter Create(Encounter encounter);
    Encounter Update(Encounter encounter);
    void Delete(long encounterId);
    Encounter GetById(long id);
    List<Encounter> GetActive();
    List<Encounter> GetAll();
    
    // Active encounter methods
    ActiveEncounter ActivateEncounter(ActiveEncounter activeEncounter);
    ActiveEncounter UpdateActiveEncounter(ActiveEncounter activeEncounter);
    List<ActiveEncounter> GetActiveByTourist(long touristId);
    List<ActiveEncounter> GetActiveByEncounter(long encounterId);
    ActiveEncounter? GetActiveTouristEncounter(long touristId, long encounterId);
    ActiveEncounter GetActiveById(long activeEncounterId);
    void DeleteActiveEncounter(long activeEncounterId);

    // Completed encounter methods
    CompletedEncounter CompleteEncounter(CompletedEncounter completedEncounter);
    bool HasCompletedEncounter(long touristId, long encounterId);
    List<CompletedEncounter> GetCompletedByTourist(long touristId);

    // Misc encounter methods
    Requirement CreateRequirement(Requirement requirement, long activeEncounterId);
    Requirement UpdateRequirement(Requirement requirement, long activeEncounterId);
    List<Requirement> GetRequirementsByActiveEncounter(long activeEncounterId);
    Requirement GetRequirementById(long id, long activeEncounterId);

}