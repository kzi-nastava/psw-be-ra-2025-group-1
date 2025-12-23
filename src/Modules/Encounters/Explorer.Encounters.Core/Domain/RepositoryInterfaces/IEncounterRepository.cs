using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.RepositoryInterfaces;

public interface IEncounterRepository
{
    Encounter Create(Encounter encounter);
    Encounter Update(Encounter encounter);
    void Delete(long encounterId);
    Encounter GetById(long id);
    List<Encounter> GetActive();
}