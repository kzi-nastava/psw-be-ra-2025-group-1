using Explorer.ProjectAutopsy.Core.Domain;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using Explorer.ProjectAutopsy.Core.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Explorer.ProjectAutopsy.Infrastructure.ExternalClients;

public class PdfExportService : IPdfExportService
{
    public PdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateRiskAnalysisPdf(string projectName, RiskSnapshot snapshot, AIReport? aiReport = null)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, projectName, snapshot));
                page.Content().Element(c => ComposeContent(c, snapshot));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, string projectName, RiskSnapshot snapshot)
    {
        container.Column(column =>
        {
            column.Item().PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Analiza GitHub Aktivnosti").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("Izveštaj o kvalitetu koda i saradnje").FontSize(14).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(120).AlignRight().Column(col =>
                {
                    col.Item().Text(snapshot.CreatedAt.ToString("dd.MM.yyyy")).FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().Text(snapshot.CreatedAt.ToString("HH:mm")).FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });

            column.Item().PaddingTop(5).PaddingBottom(15).Text(projectName).FontSize(18).Bold();
            column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Pregled podataka
            column.Item().Background(Colors.Blue.Lighten4).Padding(15).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Period analize").FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{snapshot.AnalysisWindowStart:dd.MM.yyyy} - {snapshot.AnalysisWindowEnd:dd.MM.yyyy}").FontSize(14).Bold();
                });

                row.ConstantItem(100).AlignCenter().Column(col =>
                {
                    col.Item().Text("Commitova").FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(snapshot.CommitsAnalyzed.ToString()).FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                });

                row.ConstantItem(100).AlignCenter().Column(col =>
                {
                    col.Item().Text("Pull Request-a").FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(snapshot.PullRequestsAnalyzed.ToString()).FontSize(20).Bold().FontColor(Colors.Green.Darken2);
                });
            });
        });
    }

    private void ComposeContent(IContainer container, RiskSnapshot snapshot)
    {
        var metrics = snapshot.Metrics;

        container.PaddingTop(20).Column(column =>
        {
            // Analiza Commit Poruka
            column.Item().Element(c => ComposeCommitAnalysis(c, metrics.CommitAnalysis));
            column.Item().PaddingVertical(15);

            // Analiza PR-ova
            column.Item().Element(c => ComposePrAnalysis(c, metrics.PrAnalysis));
            column.Item().PaddingVertical(15);

            // Analiza Review-a
            column.Item().Element(c => ComposeReviewAnalysis(c, metrics.ReviewAnalysis));
            column.Item().PaddingVertical(15);

            // Aktivnost po autoru
            if (metrics.AuthorActivity.Any())
            {
                column.Item().Element(c => ComposeAuthorActivity(c, metrics.AuthorActivity));
            }
        });
    }

    private void ComposeCommitAnalysis(IContainer container, CommitAnalysis analysis)
    {
        container.Column(column =>
        {
            column.Item().Text("Analiza Commit Poruka").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            // Statistika
            column.Item().Row(row =>
            {
                row.RelativeItem().Background(Colors.Green.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Dobre poruke").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{analysis.GoodMessagePercentage:F1}%").FontSize(20).Bold().FontColor(Colors.Green.Darken2);
                    col.Item().Text($"{analysis.GoodMessageCount} od {analysis.TotalCommits}").FontSize(9);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Red.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Loše poruke").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{100 - analysis.GoodMessagePercentage:F1}%").FontSize(20).Bold().FontColor(Colors.Red.Darken2);
                    col.Item().Text($"{analysis.BadMessageCount} od {analysis.TotalCommits}").FontSize(9);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Blue.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Prosek dnevno").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{analysis.AverageCommitsPerDay:F1}").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("commitova").FontSize(9);
                });
            });

            // Ocena kvaliteta
            var quality = analysis.GoodMessagePercentage switch
            {
                >= 80 => ("Odličan kvalitet commit poruka!", Colors.Green.Darken1),
                >= 60 => ("Dobar kvalitet, ali ima prostora za poboljšanje.", Colors.Orange.Darken1),
                >= 40 => ("Prosečan kvalitet - preporučuje se usvajanje konvencija.", Colors.Orange.Darken2),
                _ => ("Loš kvalitet - potrebno je uvesti standarde za commit poruke.", Colors.Red.Darken1)
            };

            column.Item().PaddingTop(10).Background(Colors.Grey.Lighten3).Padding(10)
                .Text(quality.Item1).FontSize(11).FontColor(quality.Item2);

            // Primeri loših commit poruka
            if (analysis.BadCommitExamples.Any())
            {
                column.Item().PaddingTop(15).Text("Primeri loših commit poruka:").FontSize(11).Bold();
                column.Item().PaddingTop(5);

                foreach (var example in analysis.BadCommitExamples)
                {
                    column.Item().PaddingBottom(5).Row(row =>
                    {
                        row.ConstantItem(60).Text(example.Sha).FontSize(8).FontColor(Colors.Grey.Medium);
                        row.RelativeItem().Text(example.Message).FontSize(9);
                        row.ConstantItem(80).AlignRight().Text(example.Author).FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                }

                column.Item().PaddingTop(10).Text("Preporuka: Koristite format 'tip: opis' (npr. 'feat: dodaj login', 'fix: ispravi bug')")
                    .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
            }

            // Commitovi po autoru
            if (analysis.CommitsByAuthor.Any())
            {
                column.Item().PaddingTop(15).Text("Commitovi po autoru:").FontSize(11).Bold();
                column.Item().PaddingTop(5);

                foreach (var (author, count) in analysis.CommitsByAuthor.Take(5))
                {
                    var percentage = analysis.TotalCommits > 0 ? (double)count / analysis.TotalCommits * 100 : 0;
                    column.Item().PaddingBottom(3).Row(row =>
                    {
                        row.ConstantItem(150).Text(author).FontSize(9);
                        row.RelativeItem().Height(15).Background(Colors.Grey.Lighten3).Layers(layers =>
                        {
                            layers.PrimaryLayer();
                            layers.Layer().Width((float)(percentage * 2)).Background(Colors.Blue.Medium);
                        });
                        row.ConstantItem(60).AlignRight().Text($"{count} ({percentage:F0}%)").FontSize(9);
                    });
                }
            }
        });
    }

    private void ComposePrAnalysis(IContainer container, PullRequestAnalysis analysis)
    {
        container.Column(column =>
        {
            column.Item().Text("Analiza Pull Request-a").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            // Statistika
            column.Item().Row(row =>
            {
                row.RelativeItem().Background(Colors.Green.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Spojeni (merged)").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(analysis.MergedPRs.ToString()).FontSize(20).Bold().FontColor(Colors.Green.Darken2);
                    col.Item().Text($"{analysis.MergeRate:F0}% uspešnost").FontSize(9);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Orange.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Otvoreni").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(analysis.OpenPRs.ToString()).FontSize(20).Bold().FontColor(Colors.Orange.Darken2);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Blue.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Prosek do spajanja").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{analysis.AverageMergeTimeHours:F0}h").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                });
            });

            // Kvalitet naslova
            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Kvalitet PR naslova").FontSize(11).Bold();
                    col.Item().PaddingTop(5).Row(r =>
                    {
                        r.ConstantItem(100).Text("Dobri naslovi:").FontSize(9);
                        r.RelativeItem().Text($"{analysis.GoodTitleCount} ({analysis.GoodTitlePercentage:F0}%)").FontSize(9).Bold();
                    });
                    col.Item().Row(r =>
                    {
                        r.ConstantItem(100).Text("Loši naslovi:").FontSize(9);
                        r.RelativeItem().Text($"{analysis.BadTitleCount} ({100 - analysis.GoodTitlePercentage:F0}%)").FontSize(9).Bold();
                    });
                });
            });

            // Primeri loših PR naslova
            if (analysis.BadTitleExamples.Any())
            {
                column.Item().PaddingTop(10).Text("Primeri loših PR naslova:").FontSize(10).Bold();
                foreach (var example in analysis.BadTitleExamples)
                {
                    column.Item().PaddingTop(3).Row(row =>
                    {
                        row.ConstantItem(40).Text($"#{example.Number}").FontSize(8).FontColor(Colors.Grey.Medium);
                        row.RelativeItem().Text(example.Title).FontSize(9);
                    });
                }
            }

            // PR-ovi po autoru
            if (analysis.PrsByAuthor.Any())
            {
                column.Item().PaddingTop(15).Text("PR-ovi po autoru:").FontSize(11).Bold();
                column.Item().PaddingTop(5);

                foreach (var (author, count) in analysis.PrsByAuthor.Take(5))
                {
                    column.Item().PaddingBottom(3).Row(row =>
                    {
                        row.ConstantItem(150).Text(author).FontSize(9);
                        row.RelativeItem().Text($"{count} PR-ova").FontSize(9);
                    });
                }
            }
        });
    }

    private void ComposeReviewAnalysis(IContainer container, ReviewAnalysis analysis)
    {
        container.Column(column =>
        {
            column.Item().Text("Analiza Code Review-a").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            // Statistika
            column.Item().Row(row =>
            {
                row.RelativeItem().Background(Colors.Purple.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Ukupno review-a").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(analysis.TotalReviews.ToString()).FontSize(20).Bold().FontColor(Colors.Purple.Darken2);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Teal.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Ukupno komentara").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(analysis.TotalComments.ToString()).FontSize(20).Bold().FontColor(Colors.Teal.Darken2);
                });

                row.ConstantItem(10);

                row.RelativeItem().Background(Colors.Indigo.Lighten4).Padding(12).Column(col =>
                {
                    col.Item().Text("Prosek po PR-u").FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{analysis.AverageCommentsPerPR:F1}").FontSize(20).Bold().FontColor(Colors.Indigo.Darken2);
                    col.Item().Text("komentara").FontSize(9);
                });
            });

            // Ocena kvaliteta
            column.Item().PaddingTop(10).Background(Colors.Grey.Lighten3).Padding(10)
                .Text(analysis.ReviewQualityAssessment).FontSize(11).Bold();

            // Prosečno vreme do prvog review-a
            if (analysis.AverageTimeToFirstReviewHours > 0)
            {
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.ConstantItem(180).Text("Prosečno vreme do prvog review-a:").FontSize(10);
                    row.RelativeItem().Text($"{analysis.AverageTimeToFirstReviewHours:F1} sati").FontSize(10).Bold();
                });
            }

            // PR-ovi bez review-a
            if (analysis.PrsWithoutReview > 0)
            {
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.ConstantItem(180).Text("PR-ovi bez review-a:").FontSize(10);
                    row.RelativeItem().Text($"{analysis.PrsWithoutReview}").FontSize(10).Bold().FontColor(Colors.Red.Darken1);
                });
            }

            // Top revieweri
            if (analysis.TopReviewers.Any())
            {
                column.Item().PaddingTop(15).Text("Najaktivniji revieweri:").FontSize(11).Bold();
                column.Item().PaddingTop(5);

                var rank = 1;
                foreach (var (reviewer, count) in analysis.TopReviewers)
                {
                    column.Item().PaddingBottom(3).Row(row =>
                    {
                        row.ConstantItem(25).Text($"{rank}.").FontSize(9).Bold();
                        row.RelativeItem().Text(reviewer).FontSize(9);
                        row.ConstantItem(80).AlignRight().Text($"{count} review-a").FontSize(9);
                    });
                    rank++;
                }
            }
            else
            {
                column.Item().PaddingTop(10).Text("Nema podataka o reviewerima - GitHub API ne vraća ove podatke za sve repozitorijume.")
                    .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
            }
        });
    }

    private void ComposeAuthorActivity(IContainer container, Dictionary<string, AuthorStats> authors)
    {
        container.Column(column =>
        {
            column.Item().Text("Aktivnost po Autoru").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingTop(10);

            // Tabela
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Ime
                    columns.RelativeColumn(1); // Commitovi
                    columns.RelativeColumn(1); // PR-ovi
                    columns.RelativeColumn(1); // Merged
                    columns.RelativeColumn(1.5f); // Dodato linija
                    columns.RelativeColumn(1.5f); // Obrisano linija
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Autor").FontSize(9).Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("Commitovi").FontSize(9).Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("PR-ovi").FontSize(9).Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("Merged").FontSize(9).Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("+ linija").FontSize(9).Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("- linija").FontSize(9).Bold();
                });

                // Rows
                foreach (var (name, stats) in authors.OrderByDescending(a => a.Value.CommitCount).Take(10))
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(name).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(stats.CommitCount.ToString()).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(stats.PrCount.ToString()).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(stats.MergedPrCount.ToString()).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text($"+{stats.TotalAdditions}").FontSize(9).FontColor(Colors.Green.Darken1);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text($"-{stats.TotalDeletions}").FontSize(9).FontColor(Colors.Red.Darken1);
                }
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Generisano pomoću ").FontSize(8).FontColor(Colors.Grey.Medium);
            text.Span("Project Autopsy").FontSize(8).Bold().FontColor(Colors.Blue.Darken2);
            text.Span($" | {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }
}
