using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.ProjectAutopsy.API.Dtos;
using Explorer.ProjectAutopsy.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.ProjectAutopsy;

[Authorize]
[Route("api/autopsy")]
[ApiController]
public class ProjectAutopsyController : ControllerBase
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

    [HttpGet("projects")]
    public ActionResult<List<AutopsyProjectDto>> GetProjects()
    {
        var result = _projectService.GetAll();
        return Ok(result);
    }

    [HttpGet("projects/{id:long}")]
    public ActionResult<AutopsyProjectDto> GetProject(long id)
    {
        var result = _projectService.Get(id);
        return Ok(result);
    }

    [HttpPost("projects")]
    public ActionResult<AutopsyProjectDto> CreateProject([FromBody] CreateAutopsyProjectDto dto)
    {
        var result = _projectService.Create(dto);
        return Ok(result);
    }

    [HttpPost("projects/{id:long}/analyze")]
    public async Task<ActionResult<RiskSnapshotDto>> RunAnalysis(long id)
    {
        var result = await _riskService.RunAnalysisAsync(id);
        return Ok(result);
    }

    [HttpGet("projects/{id:long}/risk")]
    public ActionResult<RiskSnapshotDto> GetLatestRisk(long id)
    {
        var result = _riskService.GetLatestSnapshot(id);
        return Ok(result);
    }
}