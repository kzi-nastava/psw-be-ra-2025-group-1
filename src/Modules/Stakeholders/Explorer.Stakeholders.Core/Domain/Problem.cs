using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum ProblemCategory
{
    Safety,
    Maintenance,
    Other
}

public enum ProblemStatus
{
    Open,
    ResolvedByTourist,
    Unresolved
}

public class Problem : Entity
{
    public long TourId { get; init; }
    public long CreatorId { get; init; }  // id turiste
    public long AuthorId { get; private set; }  
    public int Priority { get; private set; }
    public string Description { get; private set; }
    public DateTime CreationTime { get; init; }
    public ProblemCategory Category { get; private set; }
    public ProblemStatus Status { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? AdminDeadline { get; private set; }
    public string? TouristComment { get; private set; }
    
    public Problem(int priority, string description, ProblemCategory category, long tourId, long creatorId, long authorId)
    {
        if (priority < 1 || priority > 5) throw new ArgumentException("Priority must be between 1 and 5.");
        Priority = priority;
        Description = description;
        CreationTime = DateTime.UtcNow;
        Category = category;
        TourId = tourId;
        CreatorId = creatorId;
        AuthorId = authorId;
        Status = ProblemStatus.Open;
    }
    
    private Problem() { }
    
    public void Update(int priority, string description, ProblemCategory category)
    {
        if (priority < 1 || priority > 5) throw new ArgumentException("Priority must be between 1 and 5.");
        Priority = priority;
        Description = description;
        Category = category;
    }

    public void MarkAsResolvedByTourist(string? comment = null)
    {
        if (Status != ProblemStatus.Open)
            throw new InvalidOperationException("Only open problems can be marked as resolved.");
        
        Status = ProblemStatus.ResolvedByTourist;
        ResolvedAt = DateTime.UtcNow;
        TouristComment = comment;
    }

    public void MarkAsUnresolved(string? comment = null)
    {
        if (Status != ProblemStatus.Open)
            throw new InvalidOperationException("Only open problems can be marked as unresolved.");
        
        Status = ProblemStatus.Unresolved;
        ResolvedAt = DateTime.UtcNow;
        TouristComment = comment;
    }

    public void CloseByAdmin(string? comment = null)
    {
        // Admin moze da zatvori problem bez obzira na status
        Status = ProblemStatus.Unresolved;
        ResolvedAt = DateTime.UtcNow;
        TouristComment = comment ?? "Closed by administrator";
    }

    public void SetAdminDeadline(DateTime deadline)
    {
        if (deadline <= DateTime.UtcNow)
            throw new ArgumentException("Deadline must be in the future.");
        
        AdminDeadline = deadline;
    }

    public bool IsOverdue()
    {
        return Status == ProblemStatus.Open && 
               CreationTime.AddDays(5) < DateTime.UtcNow;
    }

    public bool HasMissedAdminDeadline()
    {
        return AdminDeadline.HasValue && 
               AdminDeadline.Value < DateTime.UtcNow && 
               Status == ProblemStatus.Open;
    }
}
