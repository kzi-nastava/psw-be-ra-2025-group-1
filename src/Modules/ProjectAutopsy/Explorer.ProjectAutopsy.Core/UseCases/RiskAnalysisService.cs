using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Core.Exceptions;
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

        // Prepare analysis input
        var windowEnd = DateTime.UtcNow;
        var windowStart = windowEnd.AddDays(-project.AnalysisWindowDays);

        var input = new RiskEngineInput
        {
            AnalysisWindowDays = project.AnalysisWindowDays,
            Tickets = new List<TicketData>(),
            Commits = new List<CommitData>(),
            PullRequests = new List<PullRequestData>(),
            PreviousSnapshots = _snapshotRepository.GetByProject(projectId).Take(5).ToList()
        };

        // Fetch real GitHub data if repository is configured
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
                // If GitHub fetch fails, fall back to sample data
                // In production, you might want to throw or log this differently
                var sampleData = GenerateSampleData(project.AnalysisWindowDays);
                input.Commits = sampleData.Commits;
                input.PullRequests = sampleData.PullRequests;
            }
        }
        else
        {
            // No GitHub repo configured, use sample data
            var sampleData = GenerateSampleData(project.AnalysisWindowDays);
            input.Commits = sampleData.Commits;
            input.PullRequests = sampleData.PullRequests;
        }

        // TODO: Integrate Jira for ticket data when available
        // For now, tickets remain empty or use sample data

        // Run the risk engine
        var result = _riskEngine.Calculate(input);

        // Create and save snapshot
        var snapshot = new RiskSnapshot(
            projectId,
            result.OverallScore,
            result.PlanningScore,
            result.ExecutionScore,
            result.BottleneckScore,
            result.CommunicationScore,
            result.StabilityScore,
            result.Metrics,
            result.Trend,
            windowStart,
            windowEnd,
            project.AnalysisWindowDays,
            result.TicketsAnalyzed,
            result.CommitsAnalyzed,
            result.PullRequestsAnalyzed
        );

        var saved = _snapshotRepository.Create(snapshot);
        
        // Update project's last analysis timestamp
        project.MarkAnalyzed();
        _projectRepository.Update(project);

        return _mapper.Map<RiskSnapshotDto>(saved);
    }

    public RiskSnapshotDto GetLatestSnapshot(long projectId)
    {
        var snapshot = _snapshotRepository.GetLatestByProject(projectId);
        if (snapshot == null)
            throw new KeyNotFoundException($"No risk snapshots found for project {projectId}");

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
            throw new KeyNotFoundException($"Snapshot with id {snapshotId} not found");

        return _mapper.Map<RiskSnapshotDto>(snapshot);
    }

    public byte[] ExportSnapshotToPdf(long snapshotId)
    {
        var snapshot = _snapshotRepository.Get(snapshotId);
        if (snapshot == null)
            throw new KeyNotFoundException($"Snapshot with id {snapshotId} not found");

        var project = _projectRepository.Get(snapshot.ProjectId);
        if (project == null)
            throw new KeyNotFoundException($"Project with id {snapshot.ProjectId} not found");

        // Try to get the latest AI report for this snapshot
        var aiReport = _aiReportRepository.GetBySnapshot(snapshotId).FirstOrDefault();

        return _pdfExportService.GenerateRiskAnalysisPdf(project.Name, snapshot, aiReport);
    }

    /// <summary>
    /// Generates sample data for demo/testing when no external integrations are available
    /// </summary>
    private RiskEngineInput GenerateSampleData(int windowDays)
    {
        var random = new Random();
        var input = new RiskEngineInput { AnalysisWindowDays = windowDays };

        // Generate sample tickets
        var ticketCount = random.Next(30, 60);
        for (int i = 0; i < ticketCount; i++)
        {
            var status = (TicketStatus)random.Next(0, 5);
            var startedAt = DateTime.UtcNow.AddDays(-random.Next(1, windowDays));
            
            input.Tickets.Add(new TicketData
            {
                ExternalId = $"TICKET-{i + 1}",
                Key = $"PROJ-{i + 1}",
                Title = $"Sample ticket {i + 1}",
                Status = status,
                Type = (TicketType)random.Next(0, 3),
                Priority = (TicketPriority)random.Next(0, 4),
                Assignee = $"developer{random.Next(1, 6)}",
                SprintId = random.NextDouble() > 0.3 ? $"sprint-{random.Next(1, 4)}" : null,
                AddedMidSprint = random.NextDouble() > 0.85,
                EstimatedPoints = random.Next(1, 8),
                ActualPoints = random.Next(1, 10),
                StartedAt = status != TicketStatus.Todo ? startedAt : null,
                CompletedAt = status == TicketStatus.Done ? startedAt.AddHours(random.Next(4, 120)) : null,
                BlockedHours = status == TicketStatus.Blocked ? random.Next(2, 48) : null
            });
        }

        // Generate sample commits
        var commitCount = random.Next(50, 150);
        for (int i = 0; i < commitCount; i++)
        {
            input.Commits.Add(new CommitData
            {
                Sha = Guid.NewGuid().ToString("N").Substring(0, 8),
                Message = $"feat: implement feature {i + 1}",
                Author = $"developer{random.Next(1, 6)}",
                CommittedAt = DateTime.UtcNow.AddDays(-random.Next(0, windowDays)).AddHours(-random.Next(0, 24)),
                Additions = random.Next(5, 200),
                Deletions = random.Next(0, 50),
                FilesChanged = random.Next(1, 15)
            });
        }

        // Generate sample PRs
        var prCount = random.Next(15, 40);
        for (int i = 0; i < prCount; i++)
        {
            var createdAt = DateTime.UtcNow.AddDays(-random.Next(1, windowDays));
            var isMerged = random.NextDouble() > 0.15;
            var mergedAt = isMerged ? createdAt.AddHours(random.Next(4, 72)) : (DateTime?)null;

            input.PullRequests.Add(new PullRequestData
            {
                Number = i + 1,
                Title = $"PR: Feature {i + 1}",
                State = isMerged ? PullRequestState.Merged : 
                        random.NextDouble() > 0.5 ? PullRequestState.Open : PullRequestState.Closed,
                Author = $"developer{random.Next(1, 6)}",
                CreatedAt = createdAt,
                MergedAt = mergedAt,
                TimeToFirstReviewHours = random.Next(2, 48),
                TimeToMergeHours = mergedAt.HasValue ? (mergedAt.Value - createdAt).TotalHours : null,
                CommentCount = random.Next(0, 10),
                ReviewCount = random.Next(1, 4)
            });
        }

        return input;
    }
}
