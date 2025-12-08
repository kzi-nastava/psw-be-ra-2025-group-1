namespace Explorer.Stakeholders.API.Dtos;

public enum ProblemCategory
{
    Safety,
    Maintenance,
    Other
}

public class ProblemDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long CreatorId { get; set; }
    public int Priority { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }
    public ProblemCategory Category { get; set; }
}
