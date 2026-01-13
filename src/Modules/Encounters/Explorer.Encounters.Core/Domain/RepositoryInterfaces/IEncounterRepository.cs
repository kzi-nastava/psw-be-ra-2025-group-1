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
    
    // Completed encounter methods
    CompletedEncounter CompleteEncounter(CompletedEncounter completedEncounter);
    bool HasCompletedEncounter(long touristId, long encounterId);
    List<CompletedEncounter> GetCompletedByTourist(long touristId);
}