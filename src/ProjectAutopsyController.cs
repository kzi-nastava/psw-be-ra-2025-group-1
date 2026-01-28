using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.ProjectAutopsy.API.Dtos;
using Explorer.ProjectAutopsy.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.ProjectAutopsy;

[Authorize]
[Route("api/autopsy")]
public class ProjectAutopsyController : BaseApiController
{
    private readonly IAutopsyProjectService _projectService;
    private readonly IRiskAnalysisService _riskService;

    public ProjectAutopsyController(
        IAutopsyProjectService projectService,
        IRiskAnalysisService riskService)
    {
        _projectService = projectService;
        _riskService = riskService;
    }

    #region Projects

    [HttpGet("projects")]
    public ActionResult<List<AutopsyProjectDto>> GetProjects()
    {
        var result = _projectService.GetAll();
        return CreateResponse(result);
    }

    [HttpGet("projects/{id:long}")]
    public ActionResult<AutopsyProjectDto> GetProject(long id)
    {
        var result = _projectService.Get(id);
        return CreateResponse(result);
    }

    [HttpPost("projects")]
    public ActionResult<AutopsyProjectDto> CreateProject([FromBody] CreateAutopsyProjectDto dto)
    {
        var result = _projectService.Create(dto);
        return CreateResponse(result);
    }

    [HttpPut("projects/{id:long}")]
    public ActionResult<AutopsyProjectDto> UpdateProject(long id, [FromBody] UpdateAutopsyProjectDto dto)
    {
        var result = _projectService.Update(id, dto);
        return CreateResponse(result);
    }

    [HttpDelete("projects/{id:long}")]
    public ActionResult DeleteProject(long id)
    {
        var result = _projectService.Delete(id);
        return CreateResponse(result);
    }

    [HttpPut("projects/{id:long}/github")]
    public ActionResult<AutopsyProjectDto> ConfigureGitHub(long id, [FromBody] GitHubIntegrationDto dto)
    {
        // Note: In production, you'd store the token securely
        var result = _projectService.ConfigureGitHub(id, dto.AccessToken);
        return CreateResponse(result);
    }

    #endregion

    #region Risk Analysis

    [HttpPost("projects/{id:long}/analyze")]
    public async Task<ActionResult<RiskSnapshotDto>> RunAnalysis(long id)
    {
        var result = await _riskService.RunAnalysisAsync(id);
        return CreateResponse(result);
    }

    [HttpGet("projects/{id:long}/risk")]
    public ActionResult<RiskSnapshotDto> GetLatestRisk(long id)
    {
        var result = _riskService.GetLatestSnapshot(id);
        return CreateResponse(result);
    }

    [HttpGet("projects/{id:long}/risk/history")]
    public ActionResult<RiskHistoryDto> GetRiskHistory(
        long id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = _riskService.GetHistory(id, page, pageSize);
        return CreateResponse(result);
    }

    [HttpGet("risk/{snapshotId:long}")]
    public ActionResult<RiskSnapshotDto> GetSnapshot(long snapshotId)
    {
        var result = _riskService.GetSnapshot(snapshotId);
        return CreateResponse(result);
    }

    #endregion
}
