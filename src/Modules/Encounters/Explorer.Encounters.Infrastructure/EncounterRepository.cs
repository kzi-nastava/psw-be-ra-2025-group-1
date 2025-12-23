using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;

namespace Explorer.Encounters.Infrastructure;

public class EncounterRepository : IEncounterRepository
{
    private readonly EncounterContext _context;

    public EncounterRepository(EncounterContext context)
    {
        _context = context;
    }

    public Encounter Create(Encounter encounter)
    {
        _context.Encounters.Add(encounter);
        _context.SaveChanges();
        return encounter;
    }

    public Encounter Update(Encounter encounter)
    {
        _context.Encounters.Update(encounter);
        _context.SaveChanges();
        return encounter;
    }

    public void Delete(long encounterId)
    {
        var encounter = GetById(encounterId);
        _context.Encounters.Remove(encounter);
        _context.SaveChanges();
    }

    public Encounter GetById(long id)
    {
        return _context.Encounters.FirstOrDefault(e => e.Id == id)
            ?? throw new KeyNotFoundException($"Encounter with ID {id} not found.");
    }

    public List<Encounter> GetActive()
    {
        return _context.Encounters
            .Where(e => e.Status == EncounterStatus.Active)
            .ToList();
    }
}