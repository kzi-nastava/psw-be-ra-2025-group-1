using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Xml.Linq;

namespace Explorer.Encounters.Core.Domain;

public class Encounter : AggregateRoot
{
    public string Title { get; private set; } = "";
    public string Description { get; private set; } = "";
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public int Xp { get; private set; }
    public EncounterStatus Status { get; private set; }
    public EncounterType Type { get; private set; }
    public int? RequiredPeopleCount { get; private set; }
    public List<string> Requirements { get; private set; }
    public double? Range { get; private set; }
    public string? ImagePath { get; private set; }
    public List<string>? Hints { get; private set; }

    private Encounter() 
    {
        Requirements = new List<string>();
        Hints = new List<string>();
    }
    public Encounter(string title, string description, double longitude, double latitude, int xp, EncounterType type, List<string> reqs, int? requiredPeopleCount = null, double? range = null, string? imgPath = null, List<string>? hints = null) : this()
    {
        Title = title;
        Description = description;
        Longitude = longitude;
        Latitude = latitude;
        Xp = xp;
        Type = type;
        RequiredPeopleCount = requiredPeopleCount;
        Range = range;
        Status = EncounterStatus.Draft;
        ImagePath = imgPath;
        Requirements = reqs;
        Hints = hints;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrEmpty(Title)) throw new EntityValidationException("Title cannot be empty.");
        if (string.IsNullOrEmpty(Description)) throw new EntityValidationException("Description cannot be empty.");
        if (Longitude < -180 || Longitude > 180) throw new EntityValidationException("Invalid longitude.");
        if (Latitude < -90 || Latitude > 90) throw new EntityValidationException("Invalid latitude.");
        if (Xp <= 0) throw new ArgumentException("XP must be > 0.");
        switch (Type)
        {
            case EncounterType.Location:
                if (string.IsNullOrEmpty(ImagePath))
                    throw new EntityValidationException("Location encounters must have an image.");
                break;
            case EncounterType.Social:
                if (RequiredPeopleCount == null || RequiredPeopleCount <= 0)
                    throw new EntityValidationException("Social encounters must have a positive requiredPeopleCount.");
                break;
            case EncounterType.Misc:
                if (Requirements == null || !Requirements.Any())
                    throw new EntityValidationException("Misc encounters must have at least one requirement.");
                break;
        }
    }

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

    public void UpdateSocial(string title, string description, double longitude, double latitude, int xp, EncounterType type, int requiredPeopleCount, double range)
    {
        if (Status != EncounterStatus.Draft)
            throw new InvalidOperationException("Only draft encounters can be updated.");

        Title = title;
        Description = description;
        Longitude = longitude;
        Latitude = latitude;
        Xp = xp;
        Type = EncounterType.Social;
        RequiredPeopleCount = requiredPeopleCount;
        Range = range;

        // Null out unrelated fields
        ImagePath = null;
        Hints = null;
        Requirements = new List<string>();

        Validate();
    }

    public void UpdateLocation(string title, string description, double longitude, double latitude, int xp, EncounterType type, double range, string imagePath, List<string>? hints = null)
    {
        if (Status != EncounterStatus.Draft)
            throw new InvalidOperationException("Only draft encounters can be updated.");

        Title = title;
        Description = description;
        Longitude = longitude;
        Latitude = latitude;
        Xp = xp;
        Type = EncounterType.Location;
        Range = range;
        ImagePath = imagePath;
        Hints = hints ?? new List<string>();

        // Null out unrelated fields
        RequiredPeopleCount = null;
        Requirements = new List<string>();

        Validate();
    }

    public void UpdateMisc(string title, string description, double longitude, double latitude, int xp, EncounterType type, List<string> requirements, double range)
    {
        if (Status != EncounterStatus.Draft)
            throw new InvalidOperationException("Only draft encounters can be updated.");

        Title = title;
        Description = description;
        Longitude = longitude;
        Latitude = latitude;
        Xp = xp;
        Type = EncounterType.Misc;
        Range = range;
        Requirements = requirements;

        // Null out unrelated fields
        RequiredPeopleCount = null;
        ImagePath = null;
        Hints = null;

        Validate();
    }
}