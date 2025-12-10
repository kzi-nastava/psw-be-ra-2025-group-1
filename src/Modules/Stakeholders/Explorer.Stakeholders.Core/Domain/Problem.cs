using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum ProblemCategory
{
    Safety,
    Maintenance,
    Other
}

public class Problem : Entity
{
    public long TourId { get; init; }
    public long CreatorId { get; init; }
    public int Priority { get; private set; }
    public string Description { get; private set; }
    public DateTime CreationTime { get; init; }
    public ProblemCategory Category { get; private set; }
    
    public Problem(int priority, string description, ProblemCategory category, long tourId, long creatorId)
    {
        if (priority < 1 || priority > 5) throw new ArgumentException("Priority must be between 1 and 5.");
        Priority = priority;
        Description = description;
        CreationTime = DateTime.UtcNow;
        Category = category;
        TourId = tourId;
        CreatorId = creatorId;
    }
    
    // Private constructor for EF Core
    private Problem() { }
    
    public void Update(int priority, string description, ProblemCategory category)
    {
        if (priority < 1 || priority > 5) throw new ArgumentException("Priority must be between 1 and 5.");
        Priority = priority;
        Description = description;
        Category = category;
    }
}
