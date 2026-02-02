using Explorer.ProjectAutopsy.Core.Domain;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Explorer.ProjectAutopsy.Infrastructure.ExternalClients;

/// <summary>
/// Service for exporting risk analysis reports to PDF format.
/// Generates professional reports with risk scores, AI insights, and recommendations.
/// </summary>
public class PdfExportService : IPdfExportService
{
    public PdfExportService()
    {
        // Configure QuestPDF license (Community for development)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateRiskAnalysisPdf(
        string projectName,
        RiskSnapshot snapshot,
        AIReport? aiReport = null)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, projectName, snapshot));
                page.Content().Element(c => ComposeContent(c, snapshot, aiReport));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, string projectName, RiskSnapshot snapshot)
    {
        container.Column(column =>
        {
            // Title
            column.Item().PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Project Autopsy").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("Risk Analysis Report").FontSize(16).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(100).AlignRight().Column(col =>
                {
                    col.Item().Text(snapshot.CreatedAt.ToString("yyyy-MM-dd")).FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().Text(snapshot.CreatedAt.ToString("HH:mm")).FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });

            // Project name
            column.Item().PaddingTop(5).PaddingBottom(15).Text(projectName).FontSize(18).Bold();

            // Divider
            column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Overall Score Banner
            column.Item().Background(GetRiskColor(snapshot.OverallScore)).Padding(15).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Overall Risk Score").FontSize(14).FontColor(Colors.White);
                    col.Item().Text($"{snapshot.OverallScore:F1} / 100").FontSize(32).Bold().FontColor(Colors.White);
                });

                row.ConstantItem(150).AlignRight().AlignMiddle().Column(col =>
                {
                    col.Item().Text("Trend").FontSize(11).FontColor(Colors.White);
                    col.Item().Text(snapshot.Trend).FontSize(16).Bold().FontColor(Colors.White);
                });
            });

            // Analysis window info
            column.Item().PaddingTop(10).PaddingBottom(5).Row(row =>
            {
                row.RelativeItem().Text($"Analysis Period: {snapshot.AnalysisWindowStart:MMM dd} - {snapshot.AnalysisWindowEnd:MMM dd, yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
                row.ConstantItem(150).AlignRight().Text($"{snapshot.AnalysisWindowEnd.Subtract(snapshot.AnalysisWindowStart).Days} days").FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private void ComposeContent(IContainer container, RiskSnapshot snapshot, AIReport? aiReport)
    {
        container.PaddingTop(20).Column(column =>
        {
            // Executive Summary (if AI report exists)
            if (aiReport?.Content != null)
            {
                column.Item().Element(c => ComposeExecutiveSummary(c, aiReport.Content));
                column.Item().PaddingVertical(15);
            }

            // Risk Dimensions Scores
            column.Item().Element(c => ComposeRiskDimensions(c, snapshot));
            column.Item().PaddingVertical(15);

            // Data Coverage
            column.Item().Element(c => ComposeDataCoverage(c, snapshot));
            column.Item().PaddingVertical(15);

            // Detailed Metrics
            column.Item().Element(c => ComposeDetailedMetrics(c, snapshot));

            // AI Findings & Recommendations (if available)
            if (aiReport?.Content != null)
            {
                column.Item().PageBreak();
                column.Item().Element(c => ComposeKeyFindings(c, aiReport.Content));
                column.Item().PaddingVertical(15);
                column.Item().Element(c => ComposeRecommendations(c, aiReport.Content));
            }
        });
    }

    private void ComposeExecutiveSummary(IContainer container, ReportContent content)
    {
        container.Column(column =>
        {
            column.Item().Text("Executive Summary").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10).Text(content.ExecutiveSummary).FontSize(10).LineHeight(1.5f);
        });
    }

    private void ComposeRiskDimensions(IContainer container, RiskSnapshot snapshot)
    {
        container.Column(column =>
        {
            column.Item().Text("Risk Dimensions").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            var dimensions = new[]
            {
                ("Planning", snapshot.PlanningScore, "Sprint completion, scope changes, estimation"),
                ("Execution", snapshot.ExecutionScore, "Velocity, cycle time, throughput"),
                ("Bottleneck", snapshot.BottleneckScore, "WIP limits, blocked tasks, blocked duration"),
                ("Communication", snapshot.CommunicationScore, "PR review time, merge rate, collaboration"),
                ("Stability", snapshot.StabilityScore, "Bug ratio, hotfix frequency, defect escape")
            };

            foreach (var (name, score, description) in dimensions)
            {
                column.Item().PaddingBottom(12).Row(row =>
                {
                    // Dimension name and description
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(name).FontSize(12).Bold();
                        col.Item().Text(description).FontSize(8).FontColor(Colors.Grey.Medium);
                    });

                    // Score bar
                    row.ConstantItem(200).AlignRight().AlignMiddle().Column(col =>
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Height(20).Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Stack(stack =>
                            {
                                var percentage = Math.Min(100, score);
                                stack.Item().Width((float)(percentage * 2)).Height(20).Background(GetRiskColor(score));
                            });

                            r.ConstantItem(50).AlignRight().AlignMiddle().PaddingLeft(8).Text($"{score:F1}").FontSize(11).Bold();
                        });
                    });
                });
            }
        });
    }

    private void ComposeDataCoverage(IContainer container, RiskSnapshot snapshot)
    {
        container.Column(column =>
        {
            column.Item().Text("Data Coverage").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            column.Item().Row(row =>
            {
                row.RelativeItem().Background(Colors.Blue.Lighten4).Padding(15).Column(col =>
                {
                    col.Item().Text("Commits Analyzed").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(snapshot.CommitsAnalyzed.ToString()).FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Green.Lighten4).Padding(15).Column(col =>
                {
                    col.Item().Text("Pull Requests").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(snapshot.PullRequestsAnalyzed.ToString()).FontSize(24).Bold().FontColor(Colors.Green.Darken2);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Orange.Lighten4).Padding(15).Column(col =>
                {
                    col.Item().Text("Tickets").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(snapshot.TicketsAnalyzed.ToString()).FontSize(24).Bold().FontColor(Colors.Orange.Darken2);
                });
            });
        });
    }

    private void ComposeDetailedMetrics(IContainer container, RiskSnapshot snapshot)
    {
        container.Column(column =>
        {
            column.Item().Text("Detailed Metrics").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            // Planning Metrics
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("Planning").FontSize(12).Bold().FontColor(Colors.Blue.Darken1);
                col.Item().PaddingLeft(10).PaddingTop(5).Column(metrics =>
                {
                    metrics.Item().Text($"Sprint Completion Rate: {snapshot.Metrics.Planning.SprintCompletionRate:P1}").FontSize(9);
                    metrics.Item().Text($"Scope Change Rate: {snapshot.Metrics.Planning.ScopeChangeRate:P1}").FontSize(9);
                    metrics.Item().Text($"Estimation Accuracy: {snapshot.Metrics.Planning.EstimationAccuracy:P1}").FontSize(9);
                });
            });

            // Execution Metrics
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("Execution").FontSize(12).Bold().FontColor(Colors.Blue.Darken1);
                col.Item().PaddingLeft(10).PaddingTop(5).Column(metrics =>
                {
                    metrics.Item().Text($"Velocity Consistency: {snapshot.Metrics.Execution.VelocityConsistency:F2}").FontSize(9);
                    metrics.Item().Text($"Average Cycle Time: {snapshot.Metrics.Execution.AverageCycleTimeHours:F1} hours").FontSize(9);
                    metrics.Item().Text($"Throughput: {snapshot.Metrics.Execution.ThroughputPerWeek:F1} items/week").FontSize(9);
                });
            });

            // Bottleneck Metrics
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("Bottleneck").FontSize(12).Bold().FontColor(Colors.Blue.Darken1);
                col.Item().PaddingLeft(10).PaddingTop(5).Column(metrics =>
                {
                    metrics.Item().Text($"WIP per Person: {snapshot.Metrics.Bottleneck.WipPerPerson:F1}").FontSize(9);
                    metrics.Item().Text($"Blocked Ratio: {snapshot.Metrics.Bottleneck.BlockedRatio:P1}").FontSize(9);
                    metrics.Item().Text($"Average Blocked Time: {snapshot.Metrics.Bottleneck.AvgBlockedHours:F1} hours").FontSize(9);
                });
            });

            // Communication Metrics
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("Communication").FontSize(12).Bold().FontColor(Colors.Blue.Darken1);
                col.Item().PaddingLeft(10).PaddingTop(5).Column(metrics =>
                {
                    metrics.Item().Text($"PR Review Time: {snapshot.Metrics.Communication.PrReviewTimeHours:F1} hours").FontSize(9);
                    metrics.Item().Text($"PR Merge Rate: {snapshot.Metrics.Communication.PrMergeRate:P1}").FontSize(9);
                    metrics.Item().Text($"Comment Density: {snapshot.Metrics.Communication.CommentDensity:F2} per PR").FontSize(9);
                });
            });

            // Stability Metrics
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("Stability").FontSize(12).Bold().FontColor(Colors.Blue.Darken1);
                col.Item().PaddingLeft(10).PaddingTop(5).Column(metrics =>
                {
                    metrics.Item().Text($"Bug Ratio: {snapshot.Metrics.Stability.BugRatio:P1}").FontSize(9);
                    metrics.Item().Text($"Hotfix Frequency: {snapshot.Metrics.Stability.HotfixFrequency:P1}").FontSize(9);
                    metrics.Item().Text($"Defect Escape Rate: {snapshot.Metrics.Stability.DefectEscapeRate:P1}").FontSize(9);
                });
            });
        });
    }

    private void ComposeKeyFindings(IContainer container, ReportContent content)
    {
        if (!content.KeyFindings.Any()) return;

        container.Column(column =>
        {
            column.Item().Text("Key Findings").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            foreach (var finding in content.KeyFindings)
            {
                column.Item().PaddingBottom(15).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(12).Column(col =>
                {
                    // Title with severity badge
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(70).Background(GetSeverityColor(finding.Severity)).Padding(4).AlignCenter()
                            .Text(finding.Severity.ToString()).FontSize(9).Bold().FontColor(Colors.White);
                        row.ConstantItem(10);
                        row.RelativeItem().AlignMiddle().Text(finding.Title).FontSize(11).Bold();
                    });

                    // Category
                    col.Item().PaddingTop(5).Text($"Category: {finding.Category}").FontSize(8).FontColor(Colors.Grey.Medium);

                    // Description
                    col.Item().PaddingTop(8).Text(finding.Description).FontSize(9).LineHeight(1.4f);

                    // Evidence
                    if (finding.Evidence.Any())
                    {
                        col.Item().PaddingTop(8).Column(evidence =>
                        {
                            evidence.Item().Text("Evidence:").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                            foreach (var item in finding.Evidence)
                            {
                                evidence.Item().PaddingLeft(10).Text($"• {item}").FontSize(8).FontColor(Colors.Grey.Darken1);
                            }
                        });
                    }
                });
            }
        });
    }

    private void ComposeRecommendations(IContainer container, ReportContent content)
    {
        if (!content.Recommendations.Any()) return;

        container.Column(column =>
        {
            column.Item().Text("Recommendations").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            foreach (var rec in content.Recommendations.OrderBy(r => r.Priority))
            {
                column.Item().PaddingBottom(15).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(12).Column(col =>
                {
                    // Priority and title
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(70).Background(Colors.Blue.Medium).Padding(4).AlignCenter()
                            .Text($"Priority {rec.Priority}").FontSize(9).Bold().FontColor(Colors.White);
                        row.ConstantItem(10);
                        row.ConstantItem(60).Background(GetEffortColor(rec.Effort)).Padding(4).AlignCenter()
                            .Text(rec.Effort.ToString()).FontSize(9).Bold().FontColor(Colors.White);
                        row.ConstantItem(10);
                        row.RelativeItem().AlignMiddle().Text(rec.Title).FontSize(11).Bold();
                    });

                    // Description
                    col.Item().PaddingTop(8).Text(rec.Description).FontSize(9).LineHeight(1.4f);

                    // Expected impact
                    col.Item().PaddingTop(8).Row(row =>
                    {
                        row.ConstantItem(100).Text("Expected Impact:").FontSize(8).Bold().FontColor(Colors.Green.Darken1);
                        row.RelativeItem().Text(rec.ExpectedImpact).FontSize(8).FontColor(Colors.Grey.Darken1);
                    });
                });
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Generated by ").FontSize(8).FontColor(Colors.Grey.Medium);
            text.Span("Project Autopsy").FontSize(8).Bold().FontColor(Colors.Blue.Darken2);
            text.Span($" on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }

    private string GetRiskColor(double score)
    {
        return score switch
        {
            < 25 => Colors.Green.Medium,
            < 50 => Colors.LightGreen.Medium,
            < 70 => Colors.Orange.Medium,
            _ => Colors.Red.Medium
        };
    }

    private string GetSeverityColor(Severity severity)
    {
        return severity switch
        {
            Severity.Low => Colors.Green.Medium,
            Severity.Medium => Colors.Orange.Medium,
            Severity.High => Colors.Orange.Darken2,
            Severity.Critical => Colors.Red.Medium,
            _ => Colors.Grey.Medium
        };
    }

    private string GetEffortColor(EffortLevel effort)
    {
        return effort switch
        {
            EffortLevel.Low => Colors.Green.Lighten1,
            EffortLevel.Medium => Colors.Orange.Lighten1,
            EffortLevel.High => Colors.Red.Lighten1,
            _ => Colors.Grey.Medium
        };
    }
}
