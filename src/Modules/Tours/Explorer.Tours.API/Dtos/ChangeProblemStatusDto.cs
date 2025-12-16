namespace Explorer.Tours.API.Dtos;

public enum ProblemStatusDto
{
    Open,
    ResolvedByTourist,
    Unresolved
}

public class ChangeProblemStatusDto
{
    public long ProblemId { get; set; }
    public ProblemStatusDto NewStatus { get; set; }
    public string? Comment { get; set; }
}
