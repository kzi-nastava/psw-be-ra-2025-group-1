using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Domain;

public class RiskSnapshot : Entity
{
    public long ProjectId { get; private set; }

    // Metrike (stored as JSON)
    public GitHubMetrics Metrics { get; private set; }

    // Trend
    public TrendDirection Trend { get; private set; }

    // Analysis window
    public DateTime AnalysisWindowStart { get; private set; }
    public DateTime AnalysisWindowEnd { get; private set; }
    public int AnalysisWindowDays { get; private set; }

    // Data coverage
    public int CommitsAnalyzed { get; private set; }
    public int PullRequestsAnalyzed { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public RiskSnapshot() { } // EF Core

    public RiskSnapshot(
        long projectId,
        GitHubMetrics metrics,
        TrendDirection trend,
        DateTime windowStart,
        DateTime windowEnd,
        int windowDays,
        int commits,
        int prs)
    {
        ProjectId = projectId;
        Metrics = metrics;
        Trend = trend;
        AnalysisWindowStart = windowStart;
        AnalysisWindowEnd = windowEnd;
        AnalysisWindowDays = windowDays;
        CommitsAnalyzed = commits;
        PullRequestsAnalyzed = prs;
        CreatedAt = DateTime.UtcNow;
    }
}

public enum TrendDirection
{
    Improving,
    Stable,
    Declining
}
