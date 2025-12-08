using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IProblemDeadlineRepository
{
    ProblemDeadline Create(ProblemDeadline deadline);
    ProblemDeadline? GetByProblemId(long problemId);
    ProblemDeadline? GetLatestByProblemId(long problemId);
    List<ProblemDeadline> GetExpiredDeadlines();
}
