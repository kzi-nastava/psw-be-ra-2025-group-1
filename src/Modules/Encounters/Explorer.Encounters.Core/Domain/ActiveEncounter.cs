using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class ActiveEncounter : Entity
{
    public long TouristId { get; private set; }
    public long EncounterId { get; private set; }
    public DateTime ActivationTime { get; private set; }
    public DateTime LastLocationUpdate { get; private set; }
    public double LastLatitude { get; private set; }
    public double LastLongitude { get; private set; }
    public bool IsWithinRange { get; private set; }
    public List<Requirement> Requirements { get; private set; } = new List<Requirement>();
    public List<string>? Hints { get; private set; } = new List<string>();
    public string? ImagePath { get; private set; }
    public int HintCounter { get; private set; }


    public ActiveEncounter(long touristId, long encounterId, double latitude, double longitude, List<string>? hints = null, string? imagePath = null)
    {
        TouristId = touristId;
        EncounterId = encounterId;
        ActivationTime = DateTime.UtcNow;
        LastLocationUpdate = DateTime.UtcNow;
        LastLatitude = latitude;
        LastLongitude = longitude;
        IsWithinRange = true;
        Hints = hints;
        ImagePath = imagePath;
        HintCounter = 0;
    }

    private ActiveEncounter() { }

    public void UpdateLocation(double latitude, double longitude)
    {
        LastLatitude = latitude;
        LastLongitude = longitude;
        LastLocationUpdate = DateTime.UtcNow;
    }

    public List<string> GetNextHint()
    {
        if (Hints == null || Hints.Count == 0 || HintCounter >= Hints.Count) throw new InvalidOperationException("No more hints to be displayed");

        List<string> result = [];

        for (int i = 0; i <= HintCounter; i++)
        {
            result.Add(Hints[i]);
        }
        HintCounter++;

        return result;
    }

    public void EnterRange()
    {
        IsWithinRange = true;
    }

    public void LeaveRange()
    {
        IsWithinRange = false;
    }

    public void AddRequirement(Requirement requirement)
    {
        Requirements.Add(requirement);
    }

    public void RemoveRequirement(Requirement requirement)
    {
        Requirements.Remove(requirement);
    }

    public bool AreAllRequirementsMet()
    {
        return Requirements.All(r => r.IsMet);
    }

    public Requirement UpdateRequirement(Requirement requirement)
    {
        var existingRequirement = Requirements.FirstOrDefault(r => r.Id == requirement.Id);
        if (existingRequirement == null)
        {
            throw new KeyNotFoundException($"Requirement with ID {requirement.Id} not found in ActiveEncounter {Id}.");
        }
        existingRequirement = requirement;
        return existingRequirement;
    }

    public Requirement GetRequirementById(long requirementId)
    {
        var requirement = Requirements.FirstOrDefault(r => r.Id == requirementId);
        if (requirement == null)
        {
            throw new KeyNotFoundException($"Requirement with ID {requirementId} not found in ActiveEncounter {Id}.");
        }
        return requirement;
    }
}
