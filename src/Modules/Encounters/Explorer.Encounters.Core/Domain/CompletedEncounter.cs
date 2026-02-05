using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class CompletedEncounter : Entity
{
    public long TouristId { get; private set; }
    public long EncounterId { get; private set; }
    public DateTime CompletionTime { get; private set; }
    public int XpEarned { get; private set; }

    public CompletedEncounter(long touristId, long encounterId, int xpEarned)
    {
        TouristId = touristId;
        EncounterId = encounterId;
        CompletionTime = DateTime.UtcNow;
        XpEarned = xpEarned;
    }

    private CompletedEncounter() { }
}
