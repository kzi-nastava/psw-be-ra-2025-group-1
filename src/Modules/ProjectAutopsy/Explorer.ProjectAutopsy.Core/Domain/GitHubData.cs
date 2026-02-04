namespace Explorer.ProjectAutopsy.Core.Domain;

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
