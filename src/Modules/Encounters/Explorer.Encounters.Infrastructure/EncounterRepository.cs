using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

    public List<Encounter> GetAll()
    {
        return _context.Encounters.ToList();
    }

    // Active encounter methods
    public ActiveEncounter ActivateEncounter(ActiveEncounter activeEncounter)
    {
        _context.ActiveEncounters.Add(activeEncounter);
        _context.SaveChanges();
        return activeEncounter;
    }

    public ActiveEncounter UpdateActiveEncounter(ActiveEncounter activeEncounter)
    {
        _context.ActiveEncounters.Update(activeEncounter);
        _context.SaveChanges();
        return activeEncounter;
    }

    public List<ActiveEncounter> GetActiveByTourist(long touristId)
    {
        return _context.ActiveEncounters
            .Include(b => b.Requirements)
            .Where(ae => ae.TouristId == touristId)
            .ToList();
    }

    public List<ActiveEncounter> GetActiveByEncounter(long encounterId)
    {
        return _context.ActiveEncounters
            .Include(b => b.Requirements)
            .Where(ae => ae.EncounterId == encounterId)
            .ToList();
    }

    public ActiveEncounter? GetActiveTouristEncounter(long touristId, long encounterId)
    {
        return _context.ActiveEncounters
            .Include(b => b.Requirements)
            .FirstOrDefault(ae => ae.TouristId == touristId && ae.EncounterId == encounterId);
    }

    public ActiveEncounter GetActiveById(long activeEncounterId)
    {
        return _context.ActiveEncounters
            .Include(b => b.Requirements)
            .FirstOrDefault(ae => ae.Id == activeEncounterId)
            ?? throw new KeyNotFoundException($"ActiveEncounter with ID {activeEncounterId} not found.");
    }

    public void DeleteActiveEncounter(long activeEncounterId)
    {
        var activeEncounter = _context.ActiveEncounters.Find(activeEncounterId);
        if (activeEncounter != null)
        {
            _context.ActiveEncounters.Remove(activeEncounter);
            _context.SaveChanges();
        }
    }

    // Completed encounter methods
    public CompletedEncounter CompleteEncounter(CompletedEncounter completedEncounter)
    {
        _context.CompletedEncounters.Add(completedEncounter);

        var activeEncounter = _context.ActiveEncounters.Where(ae =>
            ae.TouristId == completedEncounter.TouristId &&
            ae.EncounterId == completedEncounter.EncounterId);

        _context.ActiveEncounters.RemoveRange(activeEncounter);

        _context.SaveChanges();
        return completedEncounter;
    }

    public bool HasCompletedEncounter(long touristId, long encounterId)
    {
        return _context.CompletedEncounters
            .Any(ce => ce.TouristId == touristId && ce.EncounterId == encounterId);
    }

    public List<CompletedEncounter> GetCompletedByTourist(long touristId)
    {
        return _context.CompletedEncounters
            .Where(ce => ce.TouristId == touristId)
            .ToList();
    }

    // Misc encounter methods

    public Requirement CreateRequirement(Requirement requirement, long activeEncounterId)
    {
        var activeEncounter = GetActiveById(activeEncounterId);
        if (activeEncounter != null)
        {
            activeEncounter.AddRequirement(requirement);
            _context.Requirements.Add(requirement);
            _context.SaveChanges();
        }
        return requirement;
    }

    public Requirement UpdateRequirement(Requirement requirement, long activeEncounterId)
    {
        var activeEncounter = GetActiveById(activeEncounterId);
        activeEncounter?.UpdateRequirement(requirement);
        _context.SaveChanges();
        return requirement;
    }

    public List<Requirement> GetRequirementsByActiveEncounter(long activeEncounterId)
    {
        return GetActiveById(activeEncounterId).Requirements;
    }

    public Requirement GetRequirementById(long id, long activeEncounterId)
    {
        return GetActiveById(activeEncounterId).GetRequirementById(id);
    }
}