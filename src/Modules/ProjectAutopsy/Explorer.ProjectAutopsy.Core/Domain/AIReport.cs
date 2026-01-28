using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Domain;

public class AIReport : Entity
{
    public long ProjectId { get; private set; }
    public long RiskSnapshotId { get; private set; }
    
    public ReportStatus Status { get; private set; }
    
    // Structured content (stored as JSON)
    public ReportContent? Content { get; private set; }
    
    // LLM metadata
    public string? ModelUsed { get; private set; }
    public int? PromptTokens { get; private set; }
    public int? CompletionTokens { get; private set; }
    public int? GenerationTimeMs { get; private set; }
    
    // Error tracking
    public string? ErrorMessage { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public AIReport() { } // EF Core

    public AIReport(long projectId, long riskSnapshotId)
    {
        ProjectId = projectId;
        RiskSnapshotId = riskSnapshotId;
        Status = ReportStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void StartGenerating()
    {
        Status = ReportStatus.Generating;
    }

    public void Complete(ReportContent content, string model, int promptTokens, int completionTokens, int generationTimeMs)
    {
        Status = ReportStatus.Completed;
        Content = content;
        ModelUsed = model;
        PromptTokens = promptTokens;
        CompletionTokens = completionTokens;
        GenerationTimeMs = generationTimeMs;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string errorMessage)
    {
        Status = ReportStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
}

public enum ReportStatus
{
    Pending,
    Generating,
    Completed,
    Failed
}

public class ReportContent
{
    public string Title { get; set; } = string.Empty;
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<KeyFinding> KeyFindings { get; set; } = new();
    public List<Recommendation> Recommendations { get; set; } = new();
    public List<RiskBreakdown> RiskBreakdown { get; set; } = new();
}

public class KeyFinding
{
    public string Category { get; set; } = string.Empty;
    public Severity Severity { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Evidence { get; set; } = new();
}

public class Recommendation
{
    public int Priority { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ExpectedImpact { get; set; } = string.Empty;
    public EffortLevel Effort { get; set; }
}

public class RiskBreakdown
{
    public string Dimension { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> KeyFactors { get; set; } = new();
}

public enum Severity
{
    Low,
    Medium,
    High,
    Critical
}

public enum EffortLevel
{
    Low,
    Medium,
    High
}
