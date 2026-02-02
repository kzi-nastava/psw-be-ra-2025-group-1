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
        var m = result.Metrics;

        sb.AppendLine("Ti si senior inženjerski konsultant koji analizira zdravlje projekta.");
        sb.AppendLine("Generiši executive-level izveštaj na SRPSKOM jeziku baziran SAMO na datim metrikama.");
        sb.AppendLine();
        sb.AppendLine("KRITIČNA PRAVILA:");
        sb.AppendLine("1. NIKADA ne izmišljaj podatke - objasni samo date metrike");
        sb.AppendLine("2. Koristi executive jezik - jasan, akcioni, profesionalan");
        sb.AppendLine("3. Fokusiraj se na poslovni uticaj");
        sb.AppendLine("4. Odgovori SAMO validnim JSON-om");
        sb.AppendLine();
        sb.AppendLine($"PROJEKAT: {projectName}");
        sb.AppendLine();
        sb.AppendLine("ANALIZA COMMIT PORUKA:");
        sb.AppendLine($"- Ukupno commitova: {m.CommitAnalysis.TotalCommits}");
        sb.AppendLine($"- Dobre poruke: {m.CommitAnalysis.GoodMessagePercentage:F1}%");
        sb.AppendLine($"- Loše poruke: {m.CommitAnalysis.BadMessageCount}");
        sb.AppendLine();
        sb.AppendLine("ANALIZA PULL REQUEST-a:");
        sb.AppendLine($"- Ukupno PR-ova: {m.PrAnalysis.TotalPRs}");
        sb.AppendLine($"- Merge rate: {m.PrAnalysis.MergeRate:F1}%");
        sb.AppendLine($"- Dobri naslovi: {m.PrAnalysis.GoodTitlePercentage:F1}%");
        sb.AppendLine($"- Prosečno vreme do merge-a: {m.PrAnalysis.AverageMergeTimeHours:F1}h");
        sb.AppendLine();
        sb.AppendLine("ANALIZA REVIEW-a:");
        sb.AppendLine($"- Ukupno review-a: {m.ReviewAnalysis.TotalReviews}");
        sb.AppendLine($"- Prosek komentara po PR-u: {m.ReviewAnalysis.AverageCommentsPerPR:F1}");
        sb.AppendLine($"- PR-ovi bez review-a: {m.ReviewAnalysis.PrsWithoutReview}");
        sb.AppendLine($"- Ocena kvaliteta: {m.ReviewAnalysis.ReviewQualityAssessment}");
        sb.AppendLine();
        sb.AppendLine("JSON FORMAT:");
        sb.AppendLine(@"{
  ""title"": ""string - naslov izveštaja"",
  ""executive_summary"": ""string - 2-3 paragrafa rezimea"",
  ""key_findings"": [
    {
      ""category"": ""Commits|PullRequests|Reviews"",
      ""severity"": ""Low|Medium|High|Critical"",
      ""title"": ""string"",
      ""description"": ""string"",
      ""evidence"": [""string""]
    }
  ],
  ""recommendations"": [
    {
      ""priority"": 1,
      ""title"": ""string"",
      ""description"": ""string"",
      ""expected_impact"": ""string"",
      ""effort"": ""Low|Medium|High""
    }
  ]
}");

        return sb.ToString();
    }

    private ReportContent ParseResponse(string response)
    {
        var json = response.Trim();
        if (json.StartsWith("```json")) json = json.Substring(7);
        else if (json.StartsWith("```")) json = json.Substring(3);
        if (json.EndsWith("```")) json = json.Substring(0, json.Length - 3);
        json = json.Trim();

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var parsed = JsonSerializer.Deserialize<ReportContentJson>(json, options);
            if (parsed == null) throw new Exception("Failed to parse LLM response as JSON");

            return ConvertToReportContent(parsed);
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to parse LLM response: {ex.Message}");
        }
    }

    private ReportContent ConvertToReportContent(ReportContentJson parsed)
    {
        return new ReportContent
        {
            Title = parsed.Title ?? "Izveštaj o analizi projekta",
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
            RiskBreakdown = new List<RiskBreakdown>()
        };
    }

    private class ReportContentJson
    {
        public string? Title { get; set; }
        public string? ExecutiveSummary { get; set; }
        public List<KeyFindingJson>? KeyFindings { get; set; }
        public List<RecommendationJson>? Recommendations { get; set; }
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
}

public class GeneratedReport
{
    public ReportContent Content { get; set; } = new();
    public string ModelUsed { get; set; } = string.Empty;
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int GenerationTimeMs { get; set; }
}

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

public class MockLLMClient : ILLMClient
{
    public Task<LLMResponse> GenerateAsync(string prompt)
    {
        var mockResponse = @"{
            ""title"": ""Izveštaj o analizi GitHub aktivnosti"",
            ""executive_summary"": ""Na osnovu analiziranih metrika, projekat pokazuje prosečan kvalitet commit poruka i PR naslova. Tim redovno radi code review ali ima prostora za poboljšanje u kvalitetu komentara."",
            ""key_findings"": [
                {
                    ""category"": ""Commits"",
                    ""severity"": ""Medium"",
                    ""title"": ""Kvalitet commit poruka"",
                    ""description"": ""Određen procenat commit poruka ne prati konvencije imenovanja."",
                    ""evidence"": [""Postotak dobrih poruka ispod 80%""]
                }
            ],
            ""recommendations"": [
                {
                    ""priority"": 1,
                    ""title"": ""Uvesti konvencije za commit poruke"",
                    ""description"": ""Implementirati Conventional Commits standard (feat:, fix:, docs:, itd.)"",
                    ""expected_impact"": ""Poboljšanje čitljivosti istorije projekta"",
                    ""effort"": ""Low""
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
