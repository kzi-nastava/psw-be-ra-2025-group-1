using Explorer.BuildingBlocks.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.ProjectAutopsy.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;

public interface IAutopsyProjectRepository : ICrudRepository<AutopsyProject>
{
    AutopsyProject? GetByName(string name);
    IEnumerable<AutopsyProject> GetActive();
}

public interface IRiskSnapshotRepository : ICrudRepository<RiskSnapshot>
{
    RiskSnapshot? GetLatestByProject(long projectId);
    IEnumerable<RiskSnapshot> GetByProject(long projectId);
    IEnumerable<RiskSnapshot> GetByProjectInRange(long projectId, DateTime start, DateTime end);
}

public interface IAIReportRepository : ICrudRepository<AIReport>
{
    IEnumerable<AIReport> GetByProject(long projectId);
    AIReport? GetLatestByProject(long projectId);
    IEnumerable<AIReport> GetBySnapshot(long snapshotId);
}
