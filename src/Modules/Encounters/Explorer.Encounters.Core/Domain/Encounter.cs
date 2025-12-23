using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class Encounter : Entity
{
    public string Title { get; private set; } = "";
    public string Description { get; private set; } = "";
    public string Location { get; private set; } = "";
    public int Xp { get; private set; }
    public EncounterStatus Status { get; private set; }
    public EncounterType Type { get; private set; }

    public Encounter(string title, string description, string location, int xp, EncounterType type)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");
        if (xp <= 0)
            throw new ArgumentException("XP must be > 0.");

        Title = title;
        Description = description;
        Location = location;
        Xp = xp;
        Type = type;
        Status = EncounterStatus.Draft;
    }

    private Encounter() { }

    public void Publish() // publish = draft->active (Activate doesn't sound like a good name for this so publish it is)
    {
        if (Status != EncounterStatus.Draft)
            throw new InvalidOperationException("Only draft encounters can be published.");
        Status = EncounterStatus.Active;
    }

    public void Archive()
    {
        if (Status == EncounterStatus.Archived)
            throw new InvalidOperationException("Encounter is already archived.");
        Status = EncounterStatus.Archived;
    }

    public void Update(string title, string description, string location, int xp, EncounterType type)
    {
        if (Status != EncounterStatus.Draft)
            throw new InvalidOperationException("Only draft encounters can be updated.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");
        if (xp <= 0)
            throw new ArgumentException("XP must be greater than 0.");

        Title = title;
        Description = description;
        Location = location;
        Xp = xp;
        Type = type;
    }
}