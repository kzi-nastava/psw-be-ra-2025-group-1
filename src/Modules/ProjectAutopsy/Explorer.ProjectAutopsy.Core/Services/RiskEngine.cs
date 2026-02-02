using Explorer.ProjectAutopsy.Core.Domain;
using System.Text.RegularExpressions;

namespace Explorer.ProjectAutopsy.Core.Services;

/// <summary>
/// Analiza GitHub aktivnosti na projektu.
/// Fokusira se na commits i pull requests jer ne koristimo Jira.
/// </summary>
public class RiskEngine
{
    // Regex za konvencionalne commit poruke (feat:, fix:, docs:, etc.)
    private static readonly Regex ConventionalCommitRegex = new(
        @"^(feat|fix|docs|style|refactor|test|chore|perf|ci|build|revert)(\(.+\))?:\s.+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Regex za dobre PR naslove (počinje sa feat/fix/etc ili ima smislenu dužinu)
    private static readonly Regex GoodPrTitleRegex = new(
        @"^(feat|fix|docs|refactor|test|chore|add|update|remove|implement|improve)[\s:\-/]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public RiskEngineResult Calculate(RiskEngineInput input)
    {
        var metrics = new GitHubMetrics();

        // Analiza commit poruka
        var commitAnalysis = AnalyzeCommits(input.Commits);
        metrics.CommitAnalysis = commitAnalysis;

        // Analiza PR-ova
        var prAnalysis = AnalyzePullRequests(input.PullRequests);
        metrics.PrAnalysis = prAnalysis;

        // Analiza review-a
        var reviewAnalysis = AnalyzeReviews(input.PullRequests);
        metrics.ReviewAnalysis = reviewAnalysis;

        // Aktivnost po autoru
        metrics.AuthorActivity = CalculateAuthorActivity(input.Commits, input.PullRequests);

        // Trend
        var trend = CalculateTrend(commitAnalysis, prAnalysis, input.PreviousSnapshots);

        return new RiskEngineResult
        {
            Metrics = metrics,
            Trend = trend,
            CommitsAnalyzed = input.Commits.Count,
            PullRequestsAnalyzed = input.PullRequests.Count
        };
    }

    private CommitAnalysis AnalyzeCommits(List<CommitData> commits)
    {
        if (commits.Count == 0)
        {
            return new CommitAnalysis();
        }

        var goodCommits = commits.Where(c => IsGoodCommitMessage(c.Message)).ToList();
        var badCommits = commits.Where(c => !IsGoodCommitMessage(c.Message)).ToList();

        // Grupisanje po autoru
        var commitsByAuthor = commits
            .GroupBy(c => c.Author)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());

        return new CommitAnalysis
        {
            TotalCommits = commits.Count,
            GoodMessageCount = goodCommits.Count,
            BadMessageCount = badCommits.Count,
            GoodMessagePercentage = (double)goodCommits.Count / commits.Count * 100,
            BadCommitExamples = badCommits.Take(5).Select(c => new CommitExample
            {
                Sha = c.Sha[..Math.Min(7, c.Sha.Length)],
                Message = c.Message.Length > 80 ? c.Message[..80] + "..." : c.Message,
                Author = c.Author
            }).ToList(),
            CommitsByAuthor = commitsByAuthor,
            AverageCommitsPerDay = commits.Count / Math.Max(1, (commits.Max(c => c.CommittedAt) - commits.Min(c => c.CommittedAt)).TotalDays)
        };
    }

    private PullRequestAnalysis AnalyzePullRequests(List<PullRequestData> prs)
    {
        if (prs.Count == 0)
        {
            return new PullRequestAnalysis();
        }

        var goodTitles = prs.Where(p => IsGoodPrTitle(p.Title)).ToList();
        var badTitles = prs.Where(p => !IsGoodPrTitle(p.Title)).ToList();

        var mergedPrs = prs.Where(p => p.State == PullRequestState.Merged).ToList();
        var avgMergeTimeHours = mergedPrs.Count > 0 && mergedPrs.Any(p => p.TimeToMergeHours.HasValue)
            ? mergedPrs.Where(p => p.TimeToMergeHours.HasValue).Average(p => p.TimeToMergeHours!.Value)
            : 0;

        // PR-ovi po autoru
        var prsByAuthor = prs
            .GroupBy(p => p.Author)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());

        return new PullRequestAnalysis
        {
            TotalPRs = prs.Count,
            MergedPRs = mergedPrs.Count,
            OpenPRs = prs.Count(p => p.State == PullRequestState.Open),
            ClosedWithoutMergePRs = prs.Count(p => p.State == PullRequestState.Closed),
            GoodTitleCount = goodTitles.Count,
            BadTitleCount = badTitles.Count,
            GoodTitlePercentage = (double)goodTitles.Count / prs.Count * 100,
            BadTitleExamples = badTitles.Take(5).Select(p => new PrExample
            {
                Number = p.Number,
                Title = p.Title,
                Author = p.Author
            }).ToList(),
            AverageMergeTimeHours = avgMergeTimeHours,
            MergeRate = (double)mergedPrs.Count / prs.Count * 100,
            PrsByAuthor = prsByAuthor
        };
    }

    private ReviewAnalysis AnalyzeReviews(List<PullRequestData> prs)
    {
        if (prs.Count == 0)
        {
            return new ReviewAnalysis();
        }

        var totalReviews = prs.Sum(p => p.ReviewCount);
        var totalComments = prs.Sum(p => p.CommentCount);

        // Ko najviše reviewuje (iz ReviewerStats ako postoji)
        var reviewerStats = new Dictionary<string, int>();
        foreach (var pr in prs)
        {
            if (pr.Reviewers != null)
            {
                foreach (var reviewer in pr.Reviewers)
                {
                    if (!reviewerStats.ContainsKey(reviewer))
                        reviewerStats[reviewer] = 0;
                    reviewerStats[reviewer]++;
                }
            }
        }

        var avgReviewTime = prs.Where(p => p.TimeToFirstReviewHours.HasValue).ToList();
        var avgTimeToFirstReview = avgReviewTime.Count > 0
            ? avgReviewTime.Average(p => p.TimeToFirstReviewHours!.Value)
            : 0;

        // Ocena kvaliteta review-a na osnovu broja komentara po PR-u
        var avgCommentsPerPr = prs.Count > 0 ? (double)totalComments / prs.Count : 0;
        var reviewQuality = avgCommentsPerPr switch
        {
            < 1 => "Nedovoljan broj komentara - review-i su površni",
            < 3 => "Prosečan broj komentara - review-i su OK",
            < 6 => "Dobar broj komentara - review-i su temeljni",
            _ => "Odličan broj komentara - review-i su vrlo detaljni"
        };

        return new ReviewAnalysis
        {
            TotalReviews = totalReviews,
            TotalComments = totalComments,
            AverageCommentsPerPR = avgCommentsPerPr,
            AverageTimeToFirstReviewHours = avgTimeToFirstReview,
            ReviewQualityAssessment = reviewQuality,
            TopReviewers = reviewerStats
                .OrderByDescending(r => r.Value)
                .Take(5)
                .ToDictionary(r => r.Key, r => r.Value),
            PrsWithoutReview = prs.Count(p => p.ReviewCount == 0)
        };
    }

    private Dictionary<string, AuthorStats> CalculateAuthorActivity(List<CommitData> commits, List<PullRequestData> prs)
    {
        var authors = new Dictionary<string, AuthorStats>();

        foreach (var commit in commits)
        {
            if (!authors.ContainsKey(commit.Author))
                authors[commit.Author] = new AuthorStats { Name = commit.Author };

            authors[commit.Author].CommitCount++;
            authors[commit.Author].TotalAdditions += commit.Additions;
            authors[commit.Author].TotalDeletions += commit.Deletions;
        }

        foreach (var pr in prs)
        {
            if (!authors.ContainsKey(pr.Author))
                authors[pr.Author] = new AuthorStats { Name = pr.Author };

            authors[pr.Author].PrCount++;
            if (pr.State == PullRequestState.Merged)
                authors[pr.Author].MergedPrCount++;
        }

        return authors;
    }

    private bool IsGoodCommitMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        // Loše poruke
        var badPatterns = new[] { "fix", "update", "change", "wip", "temp", "test", "asdf", "." };
        var lowerMessage = message.ToLower().Trim();

        if (badPatterns.Any(p => lowerMessage == p))
            return false;

        if (message.Length < 10)
            return false;

        // Dobro ako prati conventional commits
        if (ConventionalCommitRegex.IsMatch(message))
            return true;

        // Dobro ako ima smislenu dužinu i ne počinje sa malim slovom bez konteksta
        if (message.Length >= 15 && !message.StartsWith("merge", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private bool IsGoodPrTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return false;

        if (title.Length < 10)
            return false;

        // Loši naslovi
        var badTitles = new[] { "fix", "update", "changes", "wip", "test", "pr" };
        if (badTitles.Any(b => title.Trim().Equals(b, StringComparison.OrdinalIgnoreCase)))
            return false;

        // Dobar ako prati pattern
        if (GoodPrTitleRegex.IsMatch(title))
            return true;

        // Dobar ako je dovoljno dug i opisan
        if (title.Length >= 20)
            return true;

        return false;
    }

    private TrendDirection CalculateTrend(CommitAnalysis commits, PullRequestAnalysis prs, List<RiskSnapshot>? previous)
    {
        // Jednostavna procena trenda na osnovu kvaliteta poruka
        var currentQuality = (commits.GoodMessagePercentage + prs.GoodTitlePercentage) / 2;

        if (previous == null || previous.Count < 2)
            return TrendDirection.Stable;

        // Uzmi prosek prethodnih
        // Za sada vraćamo Stable jer nemamo istorijske podatke u novom formatu
        return TrendDirection.Stable;
    }
}

#region Input/Output Models

public class RiskEngineInput
{
    public List<CommitData> Commits { get; set; } = new();
    public List<PullRequestData> PullRequests { get; set; } = new();
    public List<RiskSnapshot>? PreviousSnapshots { get; set; }
    public int AnalysisWindowDays { get; set; } = 30;
}

public class RiskEngineResult
{
    public GitHubMetrics Metrics { get; set; } = new();
    public TrendDirection Trend { get; set; }
    public int CommitsAnalyzed { get; set; }
    public int PullRequestsAnalyzed { get; set; }
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
    public List<string>? Reviewers { get; set; }
}

public enum PullRequestState { Open, Merged, Closed }

#endregion

#region Metrics Models

public class GitHubMetrics
{
    public CommitAnalysis CommitAnalysis { get; set; } = new();
    public PullRequestAnalysis PrAnalysis { get; set; } = new();
    public ReviewAnalysis ReviewAnalysis { get; set; } = new();
    public Dictionary<string, AuthorStats> AuthorActivity { get; set; } = new();
}

public class CommitAnalysis
{
    public int TotalCommits { get; set; }
    public int GoodMessageCount { get; set; }
    public int BadMessageCount { get; set; }
    public double GoodMessagePercentage { get; set; }
    public List<CommitExample> BadCommitExamples { get; set; } = new();
    public Dictionary<string, int> CommitsByAuthor { get; set; } = new();
    public double AverageCommitsPerDay { get; set; }
}

public class CommitExample
{
    public string Sha { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}

public class PullRequestAnalysis
{
    public int TotalPRs { get; set; }
    public int MergedPRs { get; set; }
    public int OpenPRs { get; set; }
    public int ClosedWithoutMergePRs { get; set; }
    public int GoodTitleCount { get; set; }
    public int BadTitleCount { get; set; }
    public double GoodTitlePercentage { get; set; }
    public List<PrExample> BadTitleExamples { get; set; } = new();
    public double AverageMergeTimeHours { get; set; }
    public double MergeRate { get; set; }
    public Dictionary<string, int> PrsByAuthor { get; set; } = new();
}

public class PrExample
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}

public class ReviewAnalysis
{
    public int TotalReviews { get; set; }
    public int TotalComments { get; set; }
    public double AverageCommentsPerPR { get; set; }
    public double AverageTimeToFirstReviewHours { get; set; }
    public string ReviewQualityAssessment { get; set; } = string.Empty;
    public Dictionary<string, int> TopReviewers { get; set; } = new();
    public int PrsWithoutReview { get; set; }
}

public class AuthorStats
{
    public string Name { get; set; } = string.Empty;
    public int CommitCount { get; set; }
    public int PrCount { get; set; }
    public int MergedPrCount { get; set; }
    public int TotalAdditions { get; set; }
    public int TotalDeletions { get; set; }
}

#endregion
