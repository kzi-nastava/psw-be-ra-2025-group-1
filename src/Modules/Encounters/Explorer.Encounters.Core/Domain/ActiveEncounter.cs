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

    public ActiveEncounter(long touristId, long encounterId, double latitude, double longitude)
    {
        TouristId = touristId;
        EncounterId = encounterId;
        ActivationTime = DateTime.UtcNow;
        LastLocationUpdate = DateTime.UtcNow;
        LastLatitude = latitude;
        LastLongitude = longitude;
        IsWithinRange = true;
    }

    private ActiveEncounter() { }

    public void UpdateLocation(double latitude, double longitude)
    {
        LastLatitude = latitude;
        LastLongitude = longitude;
        LastLocationUpdate = DateTime.UtcNow;
    }

    public void EnterRange()
    {
        IsWithinRange = true;
    }

    public void LeaveRange()
    {
        IsWithinRange = false;
    }
}
