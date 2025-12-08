using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class ProblemDeadline : Entity
{
    public long ProblemId { get; private set; }
    public DateTime DeadlineDate { get; private set; }
    public long SetByAdminId { get; private set; }
    public DateTime SetAt { get; private set; }

    private ProblemDeadline() { }

    public ProblemDeadline(long problemId, DateTime deadlineDate, long setByAdminId)
    {
        if (deadlineDate <= DateTime.UtcNow)
            throw new ArgumentException("Deadline must be in the future.");
        
        ProblemId = problemId;
        DeadlineDate = deadlineDate;
        SetByAdminId = setByAdminId;
        SetAt = DateTime.UtcNow;
    }

    public bool HasExpired()
    {
        return DeadlineDate < DateTime.UtcNow;
    }
}
