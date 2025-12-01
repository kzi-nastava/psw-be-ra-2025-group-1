namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IProblemMessageRepository
{
    ProblemMessage Add(ProblemMessage message);
    IEnumerable<ProblemMessage> GetByProblemId(long problemId);
    ProblemMessage? Get(long id);
}
