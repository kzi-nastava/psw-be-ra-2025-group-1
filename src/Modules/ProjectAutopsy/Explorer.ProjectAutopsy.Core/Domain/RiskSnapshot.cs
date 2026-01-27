using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Domain;

public class RiskSnapshot : Entity
{
    public long ProjectId { get; private set; }
    
    // Overall score (0-100, lower is better)
    public double OverallScore { get; private set; }
    
    // Dimension scores (0-100 each)
    public double PlanningScore { get; private set; }
    public double ExecutionScore { get; private set; }
    public double BottleneckScore { get; private set; }
    public double CommunicationScore { get; private set; }
    public double StabilityScore { get; private set; }
    
    // Raw metrics (stored as JSON)
    public RiskMetrics Metrics { get; private set; }
    
    // Trend compared to previous snapshots
    public TrendDirection Trend { get; private set; }
    
    // Analysis window
    public DateTime AnalysisWindowStart { get; private set; }
    public DateTime AnalysisWindowEnd { get; private set; }
    public int AnalysisWindowDays { get; private set; }
    
    // Data coverage
    public int TicketsAnalyzed { get; private set; }
    public int CommitsAnalyzed { get; private set; }
    public int PullRequestsAnalyzed { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    public RiskSnapshot() { } // EF Core

    public RiskSnapshot(
        long projectId,
        double overallScore,
        double planningScore,
        double executionScore,
        double bottleneckScore,
        double communicationScore,
        double stabilityScore,
        RiskMetrics metrics,
        TrendDirection trend,
        DateTime windowStart,
        DateTime windowEnd,
        int windowDays,
        int tickets,
        int commits,
        int prs)
    {
        ProjectId = projectId;
        OverallScore = overallScore;
        PlanningScore = planningScore;
        ExecutionScore = executionScore;
        BottleneckScore = bottleneckScore;
        CommunicationScore = communicationScore;
        StabilityScore = stabilityScore;
        Metrics = metrics;
        Trend = trend;
        AnalysisWindowStart = windowStart;
        AnalysisWindowEnd = windowEnd;
        AnalysisWindowDays = windowDays;
        TicketsAnalyzed = tickets;
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

public class RiskMetrics
{
    public PlanningMetrics Planning { get; set; } = new();
    public ExecutionMetrics Execution { get; set; } = new();
    public BottleneckMetrics Bottleneck { get; set; } = new();
    public CommunicationMetrics Communication { get; set; } = new();
    public StabilityMetrics Stability { get; set; } = new();
}

public class PlanningMetrics
{
    public double SprintCompletionRate { get; set; }
    public double ScopeChangeRate { get; set; }
    public double EstimationAccuracy { get; set; }
}

public class ExecutionMetrics
{
    public double VelocityConsistency { get; set; }
    public double AverageCycleTimeHours { get; set; }
    public double ThroughputPerWeek { get; set; }
}

public class BottleneckMetrics
{
    public double WipPerPerson { get; set; }
    public double BlockedRatio { get; set; }
    public double AvgBlockedHours { get; set; }
}

public class CommunicationMetrics
{
    public double PrReviewTimeHours { get; set; }
    public double PrMergeRate { get; set; }
    public double CommentDensity { get; set; }
}

public class StabilityMetrics
{
    public double BugRatio { get; set; }
    public double HotfixFrequency { get; set; }
    public double DefectEscapeRate { get; set; }
}
