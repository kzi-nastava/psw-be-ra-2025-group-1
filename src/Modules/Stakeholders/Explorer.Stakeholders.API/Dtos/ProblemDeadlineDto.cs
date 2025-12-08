namespace Explorer.Stakeholders.API.Dtos;

public class ProblemDeadlineDto
{
    public long Id { get; set; }
    public long ProblemId { get; set; }
    public DateTime DeadlineDate { get; set; }
    public long SetByAdminId { get; set; }
    public DateTime SetAt { get; set; }
    public bool HasExpired { get; set; }
}
