namespace Explorer.ProjectAutopsy.API.Dtos;

// ============================================================================
// PROJECT DTOs
// ============================================================================

public class AutopsyProjectDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
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
    public string? RepositoryUrl { get; set; }
    public int AnalysisWindowDays { get; set; } = 30;
}

public class UpdateAutopsyProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
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
    public GitHubMetricsDto Metrics { get; set; } = new();
    public string Trend { get; set; } = "Stable";
    public DateTime AnalysisWindowStart { get; set; }
    public DateTime AnalysisWindowEnd { get; set; }
    public int CommitsAnalyzed { get; set; }
    public int PullRequestsAnalyzed { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GitHubMetricsDto
{
    public CommitAnalysisDto CommitAnalysis { get; set; } = new();
    public PullRequestAnalysisDto PrAnalysis { get; set; } = new();
    public ReviewAnalysisDto ReviewAnalysis { get; set; } = new();
    public Dictionary<string, AuthorStatsDto> AuthorActivity { get; set; } = new();
}

public class CommitAnalysisDto
{
    public int TotalCommits { get; set; }
    public int GoodMessageCount { get; set; }
    public int BadMessageCount { get; set; }
    public double GoodMessagePercentage { get; set; }
    public List<CommitExampleDto> BadCommitExamples { get; set; } = new();
    public Dictionary<string, int> CommitsByAuthor { get; set; } = new();
    public double AverageCommitsPerDay { get; set; }
}

public class CommitExampleDto
{
    public string Sha { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}

public class PullRequestAnalysisDto
{
    public int TotalPRs { get; set; }
    public int MergedPRs { get; set; }
    public int OpenPRs { get; set; }
    public int ClosedWithoutMergePRs { get; set; }
    public int GoodTitleCount { get; set; }
    public int BadTitleCount { get; set; }
    public double GoodTitlePercentage { get; set; }
    public List<PrExampleDto> BadTitleExamples { get; set; } = new();
    public double AverageMergeTimeHours { get; set; }
    public double MergeRate { get; set; }
    public Dictionary<string, int> PrsByAuthor { get; set; } = new();
}

public class PrExampleDto
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}

public class ReviewAnalysisDto
{
    public int TotalReviews { get; set; }
    public int TotalComments { get; set; }
    public double AverageCommentsPerPR { get; set; }
    public double AverageTimeToFirstReviewHours { get; set; }
    public string ReviewQualityAssessment { get; set; } = string.Empty;
    public Dictionary<string, int> TopReviewers { get; set; } = new();
    public int PrsWithoutReview { get; set; }
}

public class AuthorStatsDto
{
    public string Name { get; set; } = string.Empty;
    public int CommitCount { get; set; }
    public int PrCount { get; set; }
    public int MergedPrCount { get; set; }
    public int TotalAdditions { get; set; }
    public int TotalDeletions { get; set; }
}

public class RiskHistoryDto
{
    public List<RiskSnapshotDto> Snapshots { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
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

public class IntegrationStatusDto
{
    public bool IsConnected { get; set; }
    public string? Error { get; set; }
    public DateTime? LastSyncAt { get; set; }
}
