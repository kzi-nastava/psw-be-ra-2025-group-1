using System.Text;
using System.Text.Json;
using Explorer.ProjectAutopsy.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Services;

/// <summary>
/// Generates AI-powered executive reports using LLM.
/// The LLM only EXPLAINS computed signals - it never invents data.
/// </summary>
public class AIReportGenerator
{
    private readonly ILLMClient _llmClient;

    public AIReportGenerator(ILLMClient llmClient)
    {
        _llmClient = llmClient;
    }

    public async Task<GeneratedReport> GenerateAsync(string projectName, RiskEngineResult riskResult)
    {
        var prompt = BuildPrompt(projectName, riskResult);
        
        var startTime = DateTime.UtcNow;
        var response = await _llmClient.GenerateAsync(prompt);
        var generationTimeMs = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;

        var content = ParseResponse(response.Content);

        return new GeneratedReport
        {
            Content = content,
            ModelUsed = response.Model,
            PromptTokens = response.PromptTokens,
            CompletionTokens = response.CompletionTokens,
            GenerationTimeMs = generationTimeMs
        };
    }

    private string BuildPrompt(string projectName, RiskEngineResult result)
    {
        var sb = new StringBuilder();

        sb.AppendLine("You are a senior engineering consultant analyzing project health data.");
        sb.AppendLine("Generate an executive-level risk report based ONLY on the provided metrics.");
        sb.AppendLine();
        sb.AppendLine("CRITICAL RULES:");
        sb.AppendLine("1. NEVER invent data - only explain the computed signals provided");
        sb.AppendLine("2. Use executive language - clear, actionable, professional");
        sb.AppendLine("3. Focus on business impact, not technical jargon");
        sb.AppendLine("4. Respond ONLY with valid JSON matching the schema below");
        sb.AppendLine();
        sb.AppendLine($"PROJECT: {projectName}");
        sb.AppendLine();
        sb.AppendLine("RISK SCORES (0-100, lower is better):");
        sb.AppendLine($"- Overall: {result.OverallScore}");
        sb.AppendLine($"- Planning: {result.PlanningScore}");
        sb.AppendLine($"- Execution: {result.ExecutionScore}");
        sb.AppendLine($"- Bottleneck: {result.BottleneckScore}");
        sb.AppendLine($"- Communication: {result.CommunicationScore}");
        sb.AppendLine($"- Stability: {result.StabilityScore}");
        sb.AppendLine($"- Trend: {result.Trend}");
        sb.AppendLine();
        sb.AppendLine("DETAILED METRICS:");
        sb.AppendLine($"Planning:");
        sb.AppendLine($"  - Sprint Completion Rate: {result.Metrics.Planning.SprintCompletionRate:P1}");
        sb.AppendLine($"  - Scope Change Rate: {result.Metrics.Planning.ScopeChangeRate:P1}");
        sb.AppendLine($"  - Estimation Accuracy: {result.Metrics.Planning.EstimationAccuracy:P1}");
        sb.AppendLine($"Execution:");
        sb.AppendLine($"  - Velocity Consistency: {result.Metrics.Execution.VelocityConsistency:F2}");
        sb.AppendLine($"  - Average Cycle Time: {result.Metrics.Execution.AverageCycleTimeHours:F1} hours");
        sb.AppendLine($"  - Throughput: {result.Metrics.Execution.ThroughputPerWeek:F1} tickets/week");
        sb.AppendLine($"Bottleneck:");
        sb.AppendLine($"  - WIP per Person: {result.Metrics.Bottleneck.WipPerPerson:F1}");
        sb.AppendLine($"  - Blocked Ratio: {result.Metrics.Bottleneck.BlockedRatio:P1}");
        sb.AppendLine($"  - Avg Blocked Time: {result.Metrics.Bottleneck.AvgBlockedHours:F1} hours");
        sb.AppendLine($"Communication:");
        sb.AppendLine($"  - PR Review Time: {result.Metrics.Communication.PrReviewTimeHours:F1} hours");
        sb.AppendLine($"  - PR Merge Rate: {result.Metrics.Communication.PrMergeRate:P1}");
        sb.AppendLine($"  - Comment Density: {result.Metrics.Communication.CommentDensity:F2} per PR");
        sb.AppendLine($"Stability:");
        sb.AppendLine($"  - Bug Ratio: {result.Metrics.Stability.BugRatio:P1}");
        sb.AppendLine($"  - Hotfix Frequency: {result.Metrics.Stability.HotfixFrequency:P1}");
        sb.AppendLine($"  - Defect Escape Rate: {result.Metrics.Stability.DefectEscapeRate:P1}");
        sb.AppendLine();
        sb.AppendLine($"DATA COVERAGE:");
        sb.AppendLine($"  - Tickets Analyzed: {result.TicketsAnalyzed}");
        sb.AppendLine($"  - Commits Analyzed: {result.CommitsAnalyzed}");
        sb.AppendLine($"  - Pull Requests Analyzed: {result.PullRequestsAnalyzed}");
        sb.AppendLine();
        sb.AppendLine("REQUIRED JSON OUTPUT FORMAT:");
        sb.AppendLine(@"{
  ""title"": ""string - report title"",
  ""executive_summary"": ""string - 2-3 paragraph executive summary"",
  ""key_findings"": [
    {
      ""category"": ""Planning|Execution|Bottleneck|Communication|Stability"",
      ""severity"": ""Low|Medium|High|Critical"",
      ""title"": ""string - finding title"",
      ""description"": ""string - detailed explanation"",
      ""evidence"": [""string - specific metric that supports this""]
    }
  ],
  ""recommendations"": [
    {
      ""priority"": 1,
      ""title"": ""string - recommendation title"",
      ""description"": ""string - detailed action steps"",
      ""expected_impact"": ""string - what improvement to expect"",
      ""effort"": ""Low|Medium|High""
    }
  ],
  ""risk_breakdown"": [
    {
      ""dimension"": ""Planning"",
      ""score"": 45.2,
      ""status"": ""At Risk|Healthy|Critical"",
      ""key_factors"": [""string - main contributing factors""]
    }
  ]
}");

        return sb.ToString();
    }

    private ReportContent ParseResponse(string response)
    {
        // Strip markdown code blocks if present
        var json = response.Trim();
        if (json.StartsWith("```json"))
        {
            json = json.Substring(7);
        }
        else if (json.StartsWith("```"))
        {
            json = json.Substring(3);
        }
        if (json.EndsWith("```"))
        {
            json = json.Substring(0, json.Length - 3);
        }
        json = json.Trim();

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var parsed = JsonSerializer.Deserialize<ReportContentJson>(json, options);
            
            if (parsed == null)
                throw new Exception("Failed to parse LLM response as JSON");

            return ConvertToReportContent(parsed);
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to parse LLM response: {ex.Message}. Response: {json.Substring(0, Math.Min(500, json.Length))}");
        }
    }

    private ReportContent ConvertToReportContent(ReportContentJson parsed)
    {
        return new ReportContent
        {
            Title = parsed.Title ?? "Risk Analysis Report",
            ExecutiveSummary = parsed.ExecutiveSummary ?? "",
            KeyFindings = parsed.KeyFindings?.Select(f => new KeyFinding
            {
                Category = f.Category ?? "General",
                Severity = Enum.TryParse<Severity>(f.Severity, true, out var sev) ? sev : Severity.Medium,
                Title = f.Title ?? "",
                Description = f.Description ?? "",
                Evidence = f.Evidence ?? new List<string>()
            }).ToList() ?? new List<KeyFinding>(),
            Recommendations = parsed.Recommendations?.Select(r => new Recommendation
            {
                Priority = r.Priority,
                Title = r.Title ?? "",
                Description = r.Description ?? "",
                ExpectedImpact = r.ExpectedImpact ?? "",
                Effort = Enum.TryParse<EffortLevel>(r.Effort, true, out var eff) ? eff : EffortLevel.Medium
            }).ToList() ?? new List<Recommendation>(),
            RiskBreakdown = parsed.RiskBreakdown?.Select(b => new RiskBreakdown
            {
                Dimension = b.Dimension ?? "",
                Score = b.Score,
                Status = b.Status ?? "Unknown",
                KeyFactors = b.KeyFactors ?? new List<string>()
            }).ToList() ?? new List<RiskBreakdown>()
        };
    }

    // JSON deserialization classes
    private class ReportContentJson
    {
        public string? Title { get; set; }
        public string? ExecutiveSummary { get; set; }
        public List<KeyFindingJson>? KeyFindings { get; set; }
        public List<RecommendationJson>? Recommendations { get; set; }
        public List<RiskBreakdownJson>? RiskBreakdown { get; set; }
    }

    private class KeyFindingJson
    {
        public string? Category { get; set; }
        public string? Severity { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Evidence { get; set; }
    }

    private class RecommendationJson
    {
        public int Priority { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ExpectedImpact { get; set; }
        public string? Effort { get; set; }
    }

    private class RiskBreakdownJson
    {
        public string? Dimension { get; set; }
        public double Score { get; set; }
        public string? Status { get; set; }
        public List<string>? KeyFactors { get; set; }
    }
}

public class GeneratedReport
{
    public ReportContent Content { get; set; } = new();
    public string ModelUsed { get; set; } = string.Empty;
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int GenerationTimeMs { get; set; }
}

/// <summary>
/// Abstraction for LLM API calls
/// </summary>
public interface ILLMClient
{
    Task<LLMResponse> GenerateAsync(string prompt);
}

public class LLMResponse
{
    public string Content { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
}

/// <summary>
/// Mock LLM client for testing without API calls
/// </summary>
public class MockLLMClient : ILLMClient
{
    public Task<LLMResponse> GenerateAsync(string prompt)
    {
        var mockResponse = @"{
            ""title"": ""Project Health Analysis Report"",
            ""executive_summary"": ""Based on the analyzed metrics, the project shows moderate risk levels with room for improvement in several key areas. The team is delivering features but faces challenges with code review turnaround times and estimation accuracy."",
            ""key_findings"": [
                {
                    ""category"": ""Communication"",
                    ""severity"": ""Medium"",
                    ""title"": ""PR Review Delays"",
                    ""description"": ""Code reviews are taking longer than optimal, which may slow down delivery velocity."",
                    ""evidence"": [""PR review time exceeds 24-hour target""]
                }
            ],
            ""recommendations"": [
                {
                    ""priority"": 1,
                    ""title"": ""Implement PR Review SLAs"",
                    ""description"": ""Establish a 24-hour review turnaround policy and consider adding review reminders."",
                    ""expected_impact"": ""20-30% reduction in cycle time"",
                    ""effort"": ""Low""
                }
            ],
            ""risk_breakdown"": [
                {
                    ""dimension"": ""Planning"",
                    ""score"": 35.0,
                    ""status"": ""Healthy"",
                    ""key_factors"": [""Good sprint completion rate"", ""Estimation needs improvement""]
                },
                {
                    ""dimension"": ""Execution"",
                    ""score"": 40.0,
                    ""status"": ""At Risk"",
                    ""key_factors"": [""Cycle time above target""]
                },
                {
                    ""dimension"": ""Bottleneck"",
                    ""score"": 25.0,
                    ""status"": ""Healthy"",
                    ""key_factors"": [""WIP within limits""]
                },
                {
                    ""dimension"": ""Communication"",
                    ""score"": 55.0,
                    ""status"": ""At Risk"",
                    ""key_factors"": [""PR review delays""]
                },
                {
                    ""dimension"": ""Stability"",
                    ""score"": 30.0,
                    ""status"": ""Healthy"",
                    ""key_factors"": [""Low bug ratio""]
                }
            ]
        }";

        return Task.FromResult(new LLMResponse
        {
            Content = mockResponse,
            Model = "mock-model",
            PromptTokens = 500,
            CompletionTokens = 300
        });
    }
}
