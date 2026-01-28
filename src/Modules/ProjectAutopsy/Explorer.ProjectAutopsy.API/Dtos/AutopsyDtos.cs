namespace Explorer.ProjectAutopsy.API.Dtos;

// ============================================================================
// PROJECT DTOs
// ============================================================================

public class AutopsyProjectDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? JiraProjectKey { get; set; }
    public string? GitHubRepo { get; set; }
    public int AnalysisWindowDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAnalysisAt { get; set; }
}

public class CreateAutopsyProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? JiraProjectKey { get; set; }
    public string? GitHubRepo { get; set; }
    public int AnalysisWindowDays { get; set; } = 30;
}

public class UpdateAutopsyProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? JiraProjectKey { get; set; }
    public string? GitHubRepo { get; set; }
    public int? AnalysisWindowDays { get; set; }
    public bool? IsActive { get; set; }
}

// ============================================================================
// RISK SNAPSHOT DTOs
// ============================================================================

public class RiskSnapshotDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public double OverallScore { get; set; }
    public double PlanningScore { get; set; }
    public double ExecutionScore { get; set; }
    public double BottleneckScore { get; set; }
    public double CommunicationScore { get; set; }
    public double StabilityScore { get; set; }
    public RiskMetricsDto Metrics { get; set; } = new();
    public string Trend { get; set; } = "Stable";
    public DateTime AnalysisWindowStart { get; set; }
    public DateTime AnalysisWindowEnd { get; set; }
    public int TicketsAnalyzed { get; set; }
    public int CommitsAnalyzed { get; set; }
    public int PullRequestsAnalyzed { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RiskMetricsDto
{
    public PlanningMetricsDto Planning { get; set; } = new();
    public ExecutionMetricsDto Execution { get; set; } = new();
    public BottleneckMetricsDto Bottleneck { get; set; } = new();
    public CommunicationMetricsDto Communication { get; set; } = new();
    public StabilityMetricsDto Stability { get; set; } = new();
}

public class PlanningMetricsDto
{
    public double SprintCompletionRate { get; set; }
    public double ScopeChangeRate { get; set; }
    public double EstimationAccuracy { get; set; }
}

public class ExecutionMetricsDto
{
    public double VelocityConsistency { get; set; }
    public double AverageCycleTimeHours { get; set; }
    public double ThroughputPerWeek { get; set; }
}

public class BottleneckMetricsDto
{
    public double WipPerPerson { get; set; }
    public double BlockedRatio { get; set; }
    public double AvgBlockedHours { get; set; }
}

public class CommunicationMetricsDto
{
    public double PrReviewTimeHours { get; set; }
    public double PrMergeRate { get; set; }
    public double CommentDensity { get; set; }
}

public class StabilityMetricsDto
{
    public double BugRatio { get; set; }
    public double HotfixFrequency { get; set; }
    public double DefectEscapeRate { get; set; }
}

public class RiskHistoryDto
{
    public List<RiskSnapshotDto> Snapshots { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class TriggerAnalysisResponseDto
{
    public string Message { get; set; } = string.Empty;
    public long ProjectId { get; set; }
}

// ============================================================================
// AI REPORT DTOs
// ============================================================================

public class AIReportDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long RiskSnapshotId { get; set; }
    public string Status { get; set; } = string.Empty;
    public ReportContentDto? Content { get; set; }
    public string? ModelUsed { get; set; }
    public int? PromptTokens { get; set; }
    public int? CompletionTokens { get; set; }
    public int? GenerationTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ReportContentDto
{
    public string Title { get; set; } = string.Empty;
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<KeyFindingDto> KeyFindings { get; set; } = new();
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public List<RiskBreakdownDto> RiskBreakdown { get; set; } = new();
}

public class KeyFindingDto
{
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Evidence { get; set; } = new();
}

public class RecommendationDto
{
    public int Priority { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ExpectedImpact { get; set; } = string.Empty;
    public string Effort { get; set; } = string.Empty;
}

public class RiskBreakdownDto
{
    public string Dimension { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> KeyFactors { get; set; } = new();
}

public class AIReportListDto
{
    public List<AIReportDto> Reports { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GenerateReportRequestDto
{
    public long ProjectId { get; set; }
    public long? RiskSnapshotId { get; set; }
}

// ============================================================================
// INTEGRATION DTOs
// ============================================================================

public class GitHubIntegrationDto
{
    public string AccessToken { get; set; } = string.Empty;
}

public class IntegrationStatusDto
{
    public bool IsConnected { get; set; }
    public string? Error { get; set; }
    public DateTime? LastSyncAt { get; set; }
}
