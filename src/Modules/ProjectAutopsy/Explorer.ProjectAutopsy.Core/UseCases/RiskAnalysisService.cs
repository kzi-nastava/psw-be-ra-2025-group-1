using AutoMapper;
using Explorer.ProjectAutopsy.API.Dtos;
using Explorer.ProjectAutopsy.API.Public;
using Explorer.ProjectAutopsy.Core.Domain;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using Explorer.ProjectAutopsy.Core.Services;

namespace Explorer.ProjectAutopsy.Core.UseCases;

public class RiskAnalysisService : IRiskAnalysisService
{
    private readonly IRiskSnapshotRepository _snapshotRepository;
    private readonly IAutopsyProjectRepository _projectRepository;
    private readonly IAIReportRepository _aiReportRepository;
    private readonly IGitHubDataService _gitHubDataService;
    private readonly IPdfExportService _pdfExportService;
    private readonly RiskEngine _riskEngine;
    private readonly IMapper _mapper;

    public RiskAnalysisService(
        IRiskSnapshotRepository snapshotRepository,
        IAutopsyProjectRepository projectRepository,
        IAIReportRepository aiReportRepository,
        IGitHubDataService gitHubDataService,
        IPdfExportService pdfExportService,
        IMapper mapper)
    {
        _snapshotRepository = snapshotRepository;
        _projectRepository = projectRepository;
        _aiReportRepository = aiReportRepository;
        _gitHubDataService = gitHubDataService;
        _pdfExportService = pdfExportService;
        _riskEngine = new RiskEngine();
        _mapper = mapper;
    }

    public async Task<RiskSnapshotDto> RunAnalysisAsync(long projectId)
    {
        var project = _projectRepository.Get(projectId);
        if (project == null)
            throw new KeyNotFoundException($"Project with id {projectId} not found");

        var windowEnd = DateTime.UtcNow;
        var windowStart = windowEnd.AddDays(-project.AnalysisWindowDays);

        var input = new RiskEngineInput
        {
            AnalysisWindowDays = project.AnalysisWindowDays,
            Commits = new List<CommitData>(),
            PullRequests = new List<PullRequestData>(),
            PreviousSnapshots = _snapshotRepository.GetByProject(projectId).Take(5).ToList()
        };

        // Fetch GitHub data
        if (!string.IsNullOrEmpty(project.GitHubRepo))
        {
            try
            {
                input.Commits = await _gitHubDataService.FetchCommitsAsync(
                    project.GitHubRepo,
                    windowStart,
                    windowEnd);

                input.PullRequests = await _gitHubDataService.FetchPullRequestsAsync(
                    project.GitHubRepo,
                    windowStart,
                    windowEnd);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Greška pri dohvatanju GitHub podataka: {ex.Message}", ex);
            }
        }
        else
        {
            throw new InvalidOperationException("GitHub repozitorijum nije konfigurisan za ovaj projekat.");
        }

        // Run analysis
        var result = _riskEngine.Calculate(input);

        // Create snapshot
        var snapshot = new RiskSnapshot(
            projectId,
            result.Metrics,
            result.Trend,
            windowStart,
            windowEnd,
            project.AnalysisWindowDays,
            result.CommitsAnalyzed,
            result.PullRequestsAnalyzed
        );

        var saved = _snapshotRepository.Create(snapshot);

        project.MarkAnalyzed();
        _projectRepository.Update(project);

        return _mapper.Map<RiskSnapshotDto>(saved);
    }

    public RiskSnapshotDto GetLatestSnapshot(long projectId)
    {
        var snapshot = _snapshotRepository.GetLatestByProject(projectId);
        if (snapshot == null)
            throw new KeyNotFoundException($"Nema analize za projekat {projectId}");

        return _mapper.Map<RiskSnapshotDto>(snapshot);
    }

    public RiskHistoryDto GetHistory(long projectId, int page = 1, int pageSize = 10)
    {
        var snapshots = _snapshotRepository.GetByProject(projectId);
        var total = snapshots.Count();

        var paged = snapshots
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new RiskHistoryDto
        {
            Snapshots = paged.Select(s => _mapper.Map<RiskSnapshotDto>(s)).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public RiskSnapshotDto GetSnapshot(long snapshotId)
    {
        var snapshot = _snapshotRepository.Get(snapshotId);
        if (snapshot == null)
            throw new KeyNotFoundException($"Snapshot sa id {snapshotId} nije pronađen");

        return _mapper.Map<RiskSnapshotDto>(snapshot);
    }

    public byte[] ExportSnapshotToPdf(long snapshotId)
    {
        var snapshot = _snapshotRepository.Get(snapshotId);
        if (snapshot == null)
            throw new KeyNotFoundException($"Snapshot sa id {snapshotId} nije pronađen");

        var project = _projectRepository.Get(snapshot.ProjectId);
        if (project == null)
            throw new KeyNotFoundException($"Projekat sa id {snapshot.ProjectId} nije pronađen");

        var aiReport = _aiReportRepository.GetBySnapshot(snapshotId).FirstOrDefault();

        return _pdfExportService.GenerateRiskAnalysisPdf(project.Name, snapshot, aiReport);
    }
}
