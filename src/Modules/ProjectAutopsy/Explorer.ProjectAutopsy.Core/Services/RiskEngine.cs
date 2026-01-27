using Explorer.ProjectAutopsy.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Services;

/// <summary>
/// Deterministic risk scoring engine.
/// Calculates risk scores across 5 dimensions based on project metrics.
/// 
/// SCORING: 0-100 where LOWER is BETTER (0 = no risk, 100 = critical risk)
/// 
/// Dimensions and weights:
/// - Planning (25%): Sprint completion, scope changes, estimation accuracy
/// - Execution (25%): Velocity consistency, cycle time, throughput
/// - Bottleneck (20%): WIP limits, blocked ratio, blocked duration
/// - Communication (15%): PR review time, merge rate, comment density
/// - Stability (15%): Bug ratio, hotfix frequency, defect escape rate
/// </summary>
public class RiskEngine
{
    // Dimension weights (must sum to 1.0)
    private const double PlanningWeight = 0.25;
    private const double ExecutionWeight = 0.25;
    private const double BottleneckWeight = 0.20;
    private const double CommunicationWeight = 0.15;
    private const double StabilityWeight = 0.15;

    // Planning thresholds
    private const double TargetSprintCompletion = 0.85;
    private const double MaxScopeChange = 0.15;
    private const double TargetEstimationAccuracy = 0.80;

    // Execution thresholds
    private const double MaxVelocityVariance = 0.25;
    private const double TargetCycleTimeHours = 72;
    private const double MinThroughputPerWeek = 5;

    // Bottleneck thresholds
    private const double MaxWipPerPerson = 3;
    private const double MaxBlockedRatio = 0.10;
    private const double MaxBlockedHours = 24;

    // Communication thresholds
    private const double TargetPrReviewHours = 24;
    private const double TargetPrMergeRate = 0.90;
    private const double MinCommentDensity = 0.5;

    // Stability thresholds
    private const double MaxBugRatio = 0.20;
    private const double MaxHotfixFrequency = 0.10;
    private const double MaxDefectEscapeRate = 0.05;

    public RiskEngineResult Calculate(RiskEngineInput input)
    {
        var metrics = new RiskMetrics();

        // Calculate Planning dimension
        var planningMetrics = CalculatePlanningMetrics(input);
        metrics.Planning = planningMetrics;
        var planningScore = CalculatePlanningScore(planningMetrics);

        // Calculate Execution dimension
        var executionMetrics = CalculateExecutionMetrics(input);
        metrics.Execution = executionMetrics;
        var executionScore = CalculateExecutionScore(executionMetrics);

        // Calculate Bottleneck dimension
        var bottleneckMetrics = CalculateBottleneckMetrics(input);
        metrics.Bottleneck = bottleneckMetrics;
        var bottleneckScore = CalculateBottleneckScore(bottleneckMetrics);

        // Calculate Communication dimension
        var communicationMetrics = CalculateCommunicationMetrics(input);
        metrics.Communication = communicationMetrics;
        var communicationScore = CalculateCommunicationScore(communicationMetrics);

        // Calculate Stability dimension
        var stabilityMetrics = CalculateStabilityMetrics(input);
        metrics.Stability = stabilityMetrics;
        var stabilityScore = CalculateStabilityScore(stabilityMetrics);

        // Calculate weighted overall score
        var overallScore = 
            planningScore * PlanningWeight +
            executionScore * ExecutionWeight +
            bottleneckScore * BottleneckWeight +
            communicationScore * CommunicationWeight +
            stabilityScore * StabilityWeight;

        // Determine trend
        var trend = CalculateTrend(overallScore, input.PreviousSnapshots);

        return new RiskEngineResult
        {
            OverallScore = Math.Round(overallScore, 2),
            PlanningScore = Math.Round(planningScore, 2),
            ExecutionScore = Math.Round(executionScore, 2),
            BottleneckScore = Math.Round(bottleneckScore, 2),
            CommunicationScore = Math.Round(communicationScore, 2),
            StabilityScore = Math.Round(stabilityScore, 2),
            Metrics = metrics,
            Trend = trend,
            TicketsAnalyzed = input.Tickets.Count,
            CommitsAnalyzed = input.Commits.Count,
            PullRequestsAnalyzed = input.PullRequests.Count
        };
    }

    #region Planning Calculations

    private PlanningMetrics CalculatePlanningMetrics(RiskEngineInput input)
    {
        var tickets = input.Tickets;

        // Sprint completion rate: completed / committed
        var sprintTickets = tickets.Where(t => !string.IsNullOrEmpty(t.SprintId)).ToList();
        var completedInSprint = sprintTickets.Count(t => t.Status == TicketStatus.Done);
        var sprintCompletionRate = sprintTickets.Count > 0 
            ? (double)completedInSprint / sprintTickets.Count 
            : 1.0;

        // Scope change rate: tickets added mid-sprint / total sprint tickets
        var addedMidSprint = sprintTickets.Count(t => t.AddedMidSprint);
        var scopeChangeRate = sprintTickets.Count > 0 
            ? (double)addedMidSprint / sprintTickets.Count 
            : 0.0;

        // Estimation accuracy: actual / estimated (closer to 1.0 is better)
        var estimatedPoints = tickets.Where(t => t.EstimatedPoints > 0).Sum(t => t.EstimatedPoints);
        var actualPoints = tickets.Where(t => t.ActualPoints > 0).Sum(t => t.ActualPoints);
        var estimationAccuracy = estimatedPoints > 0 
            ? Math.Min((double)actualPoints / estimatedPoints, 2.0) 
            : 1.0;

        return new PlanningMetrics
        {
            SprintCompletionRate = sprintCompletionRate,
            ScopeChangeRate = scopeChangeRate,
            EstimationAccuracy = estimationAccuracy
        };
    }

    private double CalculatePlanningScore(PlanningMetrics m)
    {
        var completionScore = ScoreAgainstTarget(m.SprintCompletionRate, TargetSprintCompletion, higherIsBetter: true);
        var scopeScore = ScoreAgainstTarget(m.ScopeChangeRate, MaxScopeChange, higherIsBetter: false);
        var estimationScore = ScoreAgainstTarget(Math.Abs(1.0 - m.EstimationAccuracy), 1 - TargetEstimationAccuracy, higherIsBetter: false);

        return completionScore * 0.4 + scopeScore * 0.3 + estimationScore * 0.3;
    }

    #endregion

    #region Execution Calculations

    private ExecutionMetrics CalculateExecutionMetrics(RiskEngineInput input)
    {
        var tickets = input.Tickets;

        // Weekly throughput
        var weeks = Math.Max(1, input.AnalysisWindowDays / 7.0);
        var completedTickets = tickets.Count(t => t.Status == TicketStatus.Done);
        var throughputPerWeek = completedTickets / weeks;

        // Average cycle time (hours from start to completion)
        var cycleTimesHours = tickets
            .Where(t => t.StartedAt.HasValue && t.CompletedAt.HasValue)
            .Select(t => (t.CompletedAt!.Value - t.StartedAt!.Value).TotalHours)
            .ToList();
        var avgCycleTimeHours = cycleTimesHours.Count > 0 ? cycleTimesHours.Average() : 0;

        // Velocity consistency (standard deviation of weekly throughput)
        var weeklyThroughputs = CalculateWeeklyThroughputs(tickets, input.AnalysisWindowDays);
        var velocityConsistency = weeklyThroughputs.Count > 1 
            ? CalculateStdDev(weeklyThroughputs) / (weeklyThroughputs.Average() + 0.01)
            : 0;

        return new ExecutionMetrics
        {
            VelocityConsistency = velocityConsistency,
            AverageCycleTimeHours = avgCycleTimeHours,
            ThroughputPerWeek = throughputPerWeek
        };
    }

    private double CalculateExecutionScore(ExecutionMetrics m)
    {
        var velocityScore = ScoreAgainstTarget(m.VelocityConsistency, MaxVelocityVariance, higherIsBetter: false);
        var cycleTimeScore = ScoreAgainstTarget(m.AverageCycleTimeHours, TargetCycleTimeHours, higherIsBetter: false);
        var throughputScore = ScoreAgainstTarget(m.ThroughputPerWeek, MinThroughputPerWeek, higherIsBetter: true);

        return velocityScore * 0.35 + cycleTimeScore * 0.35 + throughputScore * 0.30;
    }

    #endregion

    #region Bottleneck Calculations

    private BottleneckMetrics CalculateBottleneckMetrics(RiskEngineInput input)
    {
        var tickets = input.Tickets;

        // WIP per person
        var inProgressTickets = tickets.Count(t => t.Status == TicketStatus.InProgress);
        var uniqueAssignees = tickets.Where(t => !string.IsNullOrEmpty(t.Assignee)).Select(t => t.Assignee).Distinct().Count();
        var wipPerPerson = uniqueAssignees > 0 ? (double)inProgressTickets / uniqueAssignees : 0;

        // Blocked ratio
        var blockedTickets = tickets.Count(t => t.Status == TicketStatus.Blocked);
        var blockedRatio = tickets.Count > 0 ? (double)blockedTickets / tickets.Count : 0;

        // Average blocked duration (hours)
        var blockedDurations = tickets
            .Where(t => t.BlockedHours.HasValue)
            .Select(t => t.BlockedHours!.Value)
            .ToList();
        var avgBlockedHours = blockedDurations.Count > 0 ? blockedDurations.Average() : 0;

        return new BottleneckMetrics
        {
            WipPerPerson = wipPerPerson,
            BlockedRatio = blockedRatio,
            AvgBlockedHours = avgBlockedHours
        };
    }

    private double CalculateBottleneckScore(BottleneckMetrics m)
    {
        var wipScore = ScoreAgainstTarget(m.WipPerPerson, MaxWipPerPerson, higherIsBetter: false);
        var blockedRatioScore = ScoreAgainstTarget(m.BlockedRatio, MaxBlockedRatio, higherIsBetter: false);
        var blockedDurationScore = ScoreAgainstTarget(m.AvgBlockedHours, MaxBlockedHours, higherIsBetter: false);

        return wipScore * 0.35 + blockedRatioScore * 0.35 + blockedDurationScore * 0.30;
    }

    #endregion

    #region Communication Calculations

    private CommunicationMetrics CalculateCommunicationMetrics(RiskEngineInput input)
    {
        var prs = input.PullRequests;

        // Average PR review time (hours)
        var reviewTimes = prs
            .Where(p => p.TimeToFirstReviewHours.HasValue)
            .Select(p => p.TimeToFirstReviewHours!.Value)
            .ToList();
        var avgReviewTimeHours = reviewTimes.Count > 0 ? reviewTimes.Average() : 0;

        // PR merge rate
        var mergedPrs = prs.Count(p => p.State == PullRequestState.Merged);
        var prMergeRate = prs.Count > 0 ? (double)mergedPrs / prs.Count : 1.0;

        // Comment density (comments per PR)
        var totalComments = prs.Sum(p => p.CommentCount);
        var commentDensity = prs.Count > 0 ? (double)totalComments / prs.Count : 0;

        return new CommunicationMetrics
        {
            PrReviewTimeHours = avgReviewTimeHours,
            PrMergeRate = prMergeRate,
            CommentDensity = commentDensity
        };
    }

    private double CalculateCommunicationScore(CommunicationMetrics m)
    {
        var reviewTimeScore = ScoreAgainstTarget(m.PrReviewTimeHours, TargetPrReviewHours, higherIsBetter: false);
        var mergeRateScore = ScoreAgainstTarget(m.PrMergeRate, TargetPrMergeRate, higherIsBetter: true);
        var commentScore = ScoreAgainstTarget(m.CommentDensity, MinCommentDensity, higherIsBetter: true);

        return reviewTimeScore * 0.40 + mergeRateScore * 0.30 + commentScore * 0.30;
    }

    #endregion

    #region Stability Calculations

    private StabilityMetrics CalculateStabilityMetrics(RiskEngineInput input)
    {
        var tickets = input.Tickets;

        // Bug ratio
        var bugs = tickets.Count(t => t.Type == TicketType.Bug);
        var bugRatio = tickets.Count > 0 ? (double)bugs / tickets.Count : 0;

        // Hotfix frequency (critical/urgent bugs)
        var hotfixes = tickets.Count(t => 
            t.Type == TicketType.Bug && 
            (t.Priority == TicketPriority.Critical || t.Priority == TicketPriority.High));
        var hotfixFrequency = tickets.Count > 0 ? (double)hotfixes / tickets.Count : 0;

        // Defect escape rate (bugs found in production - approximated by high priority bugs)
        var escapedDefects = tickets.Count(t => 
            t.Type == TicketType.Bug && 
            t.Priority == TicketPriority.Critical);
        var defectEscapeRate = tickets.Count > 0 ? (double)escapedDefects / tickets.Count : 0;

        return new StabilityMetrics
        {
            BugRatio = bugRatio,
            HotfixFrequency = hotfixFrequency,
            DefectEscapeRate = defectEscapeRate
        };
    }

    private double CalculateStabilityScore(StabilityMetrics m)
    {
        var bugScore = ScoreAgainstTarget(m.BugRatio, MaxBugRatio, higherIsBetter: false);
        var hotfixScore = ScoreAgainstTarget(m.HotfixFrequency, MaxHotfixFrequency, higherIsBetter: false);
        var defectScore = ScoreAgainstTarget(m.DefectEscapeRate, MaxDefectEscapeRate, higherIsBetter: false);

        return bugScore * 0.40 + hotfixScore * 0.35 + defectScore * 0.25;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Scores a value against a target using sigmoid-like scaling.
    /// Returns 0-100 where 0 = meeting target, 100 = far from target.
    /// </summary>
    private double ScoreAgainstTarget(double value, double target, bool higherIsBetter)
    {
        double ratio;
        
        if (higherIsBetter)
        {
            // For metrics where higher is better (e.g., completion rate)
            ratio = target > 0 ? value / target : 1.0;
            // Score decreases as value approaches/exceeds target
            return Math.Max(0, Math.Min(100, 100 * (1 - ratio)));
        }
        else
        {
            // For metrics where lower is better (e.g., bug ratio)
            ratio = target > 0 ? value / target : 0;
            // Score increases as value exceeds target
            return Math.Max(0, Math.Min(100, 100 * ratio));
        }
    }

    private TrendDirection CalculateTrend(double currentScore, List<RiskSnapshot>? previousSnapshots)
    {
        if (previousSnapshots == null || previousSnapshots.Count < 2)
            return TrendDirection.Stable;

        var recentScores = previousSnapshots
            .OrderByDescending(s => s.CreatedAt)
            .Take(3)
            .Select(s => s.OverallScore)
            .ToList();

        if (recentScores.Count == 0)
            return TrendDirection.Stable;

        var averagePrevious = recentScores.Average();
        var delta = currentScore - averagePrevious;

        if (delta <= -5) return TrendDirection.Improving;  // Score decreased (better)
        if (delta >= 5) return TrendDirection.Declining;   // Score increased (worse)
        return TrendDirection.Stable;
    }

    private List<double> CalculateWeeklyThroughputs(List<TicketData> tickets, int windowDays)
    {
        var throughputs = new List<double>();
        var startDate = DateTime.UtcNow.AddDays(-windowDays);

        for (int week = 0; week < windowDays / 7; week++)
        {
            var weekStart = startDate.AddDays(week * 7);
            var weekEnd = weekStart.AddDays(7);

            var completed = tickets.Count(t => 
                t.CompletedAt.HasValue && 
                t.CompletedAt >= weekStart && 
                t.CompletedAt < weekEnd);

            throughputs.Add(completed);
        }

        return throughputs;
    }

    private double CalculateStdDev(List<double> values)
    {
        if (values.Count < 2) return 0;
        
        var avg = values.Average();
        var sumSquares = values.Sum(v => Math.Pow(v - avg, 2));
        return Math.Sqrt(sumSquares / (values.Count - 1));
    }

    #endregion
}

#region Input/Output Models

public class RiskEngineInput
{
    public List<TicketData> Tickets { get; set; } = new();
    public List<CommitData> Commits { get; set; } = new();
    public List<PullRequestData> PullRequests { get; set; } = new();
    public List<RiskSnapshot>? PreviousSnapshots { get; set; }
    public int AnalysisWindowDays { get; set; } = 30;
}

public class RiskEngineResult
{
    public double OverallScore { get; set; }
    public double PlanningScore { get; set; }
    public double ExecutionScore { get; set; }
    public double BottleneckScore { get; set; }
    public double CommunicationScore { get; set; }
    public double StabilityScore { get; set; }
    public RiskMetrics Metrics { get; set; } = new();
    public TrendDirection Trend { get; set; }
    public int TicketsAnalyzed { get; set; }
    public int CommitsAnalyzed { get; set; }
    public int PullRequestsAnalyzed { get; set; }
}

public class TicketData
{
    public string ExternalId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketType Type { get; set; }
    public TicketPriority Priority { get; set; }
    public string? Assignee { get; set; }
    public string? SprintId { get; set; }
    public bool AddedMidSprint { get; set; }
    public double EstimatedPoints { get; set; }
    public double ActualPoints { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double? BlockedHours { get; set; }
}

public class CommitData
{
    public string Sha { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CommittedAt { get; set; }
    public int Additions { get; set; }
    public int Deletions { get; set; }
    public int FilesChanged { get; set; }
}

public class PullRequestData
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public PullRequestState State { get; set; }
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? MergedAt { get; set; }
    public double? TimeToFirstReviewHours { get; set; }
    public double? TimeToMergeHours { get; set; }
    public int CommentCount { get; set; }
    public int ReviewCount { get; set; }
}

public enum TicketStatus { Todo, InProgress, InReview, Blocked, Done }
public enum TicketType { Story, Task, Bug, Epic, Subtask }
public enum TicketPriority { Critical, High, Medium, Low }
public enum PullRequestState { Open, Merged, Closed }

#endregion
