namespace Explorer.Stakeholders.API.Dtos;

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

public class ProblemDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long CreatorId { get; set; }  // id turiste
    public long AuthorId { get; set; }   
    public int Priority { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }
    public ProblemCategory Category { get; set; }
    public ProblemStatus Status { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? AdminDeadline { get; set; }
    public string? TouristComment { get; set; }
    public bool LateFlag { get; set; }  // True ako je problem stariji od 5 dana i nije resen
}
