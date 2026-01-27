using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.ProjectAutopsy.API.Dtos;

namespace Explorer.ProjectAutopsy.API.Public;

public interface IAutopsyProjectService
{
    AutopsyProjectDto Create(CreateAutopsyProjectDto dto);
    AutopsyProjectDto Update(long id, UpdateAutopsyProjectDto dto);
    void Delete(long id);
    AutopsyProjectDto Get(long id);
    List<AutopsyProjectDto> GetAll();
    AutopsyProjectDto ConfigureGitHub(long id, string repo);
}

public interface IRiskAnalysisService
{
    Task<RiskSnapshotDto> RunAnalysisAsync(long projectId);
    RiskSnapshotDto GetLatestSnapshot(long projectId);
    RiskHistoryDto GetHistory(long projectId, int page = 1, int pageSize = 10);
    RiskSnapshotDto GetSnapshot(long snapshotId);
}