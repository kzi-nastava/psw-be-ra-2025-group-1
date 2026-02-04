using System.Net.Http.Headers;
using System.Text.Json;
using Explorer.ProjectAutopsy.Core.Services;

namespace Explorer.ProjectAutopsy.Infrastructure.ExternalClients;

/// <summary>
/// Client for fetching data from GitHub API.
/// Retrieves commits and pull requests for risk analysis.
/// </summary>
public class GitHubClient
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
    private const string BaseUrl = "https://api.github.com";

    public GitHubClient(string accessToken, HttpClient? httpClient = null)
    {
        _accessToken = accessToken;
        _httpClient = httpClient ?? new HttpClient();
        
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ProjectAutopsy", "1.0"));
        
        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    /// <summary>
    /// Validates the connection and token
    /// </summary>
    public async Task<bool> ValidateConnectionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/user");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Fetches commits from a repository
    /// </summary>
    /// <param name="owner">Repository owner</param>
    /// <param name="repo">Repository name</param>
    /// <param name="since">Fetch commits since this date</param>
    /// <param name="maxResults">Maximum number of commits to fetch</param>
    public async Task<List<CommitData>> FetchCommitsAsync(string owner, string repo, DateTime? since = null, int maxResults = 100)
    {
        var commits = new List<CommitData>();
        var page = 1;
        var perPage = Math.Min(100, maxResults);

        while (commits.Count < maxResults)
        {
            var url = $"/repos/{owner}/{repo}/commits?page={page}&per_page={perPage}";
            if (since.HasValue)
            {
                url += $"&since={since.Value:yyyy-MM-ddTHH:mm:ssZ}";
            }

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new GitHubAuthException("GitHub authentication failed");
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    throw new GitHubRateLimitException("GitHub rate limit exceeded");
                throw new GitHubApiException($"GitHub API error: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var pageCommits = JsonSerializer.Deserialize<List<GitHubCommit>>(content, JsonOptions);

            if (pageCommits == null || pageCommits.Count == 0)
                break;

            foreach (var commit in pageCommits)
            {
                commits.Add(new CommitData
                {
                    Sha = commit.Sha ?? "",
                    Message = commit.Commit?.Message ?? "",
                    Author = commit.Commit?.Author?.Name ?? commit.Author?.Login ?? "unknown",
                    CommittedAt = commit.Commit?.Author?.Date ?? DateTime.UtcNow,
                    Additions = commit.Stats?.Additions ?? 0,
                    Deletions = commit.Stats?.Deletions ?? 0,
                    FilesChanged = commit.Files?.Count ?? 0
                });

                if (commits.Count >= maxResults)
                    break;
            }

            if (pageCommits.Count < perPage)
                break;

            page++;
            
            // Safety limit
            if (page > 10) break;
        }

        return commits;
    }

    /// <summary>
    /// Fetches pull requests from a repository
    /// </summary>
    public async Task<List<PullRequestData>> FetchPullRequestsAsync(string owner, string repo, DateTime? since = null, int maxResults = 100)
    {
        var prs = new List<PullRequestData>();
        var page = 1;
        var perPage = Math.Min(100, maxResults);

        while (prs.Count < maxResults)
        {
            var url = $"/repos/{owner}/{repo}/pulls?state=all&sort=updated&direction=desc&page={page}&per_page={perPage}";

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new GitHubAuthException("GitHub authentication failed");
                throw new GitHubApiException($"GitHub API error: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var pagePrs = JsonSerializer.Deserialize<List<GitHubPullRequest>>(content, JsonOptions);

            if (pagePrs == null || pagePrs.Count == 0)
                break;

            foreach (var pr in pagePrs)
            {
                // Filter by date if specified
                if (since.HasValue && pr.UpdatedAt < since.Value)
                    continue;

                var prData = new PullRequestData
                {
                    Number = pr.Number,
                    Title = pr.Title ?? "",
                    State = MapPrState(pr.State, pr.MergedAt),
                    Author = pr.User?.Login ?? "unknown",
                    CreatedAt = pr.CreatedAt,
                    MergedAt = pr.MergedAt
                };

                // Calculate time to first review (would need additional API call for accuracy)
                if (pr.CreatedAt != default && pr.MergedAt.HasValue)
                {
                    prData.TimeToMergeHours = (pr.MergedAt.Value - pr.CreatedAt).TotalHours;
                    // Estimate first review as 30% of merge time (simplification)
                    prData.TimeToFirstReviewHours = prData.TimeToMergeHours * 0.3;
                }

                // Get comment count from separate endpoint would be needed for accuracy
                prData.CommentCount = pr.Comments ?? 0;
                prData.ReviewCount = pr.ReviewComments ?? 0;

                prs.Add(prData);

                if (prs.Count >= maxResults)
                    break;
            }

            if (pagePrs.Count < perPage)
                break;

            page++;
            
            if (page > 10) break;
        }

        return prs;
    }

    /// <summary>
    /// Parses owner/repo from a repository string.
    /// Supports both "owner/repo" format and full URLs like "https://github.com/owner/repo"
    /// </summary>
    public static (string owner, string repo) ParseRepoString(string repoString)
    {
        var input = repoString.Trim();

        // Handle full GitHub URLs
        if (input.StartsWith("https://github.com/", StringComparison.OrdinalIgnoreCase))
        {
            input = input.Substring("https://github.com/".Length);
        }
        else if (input.StartsWith("http://github.com/", StringComparison.OrdinalIgnoreCase))
        {
            input = input.Substring("http://github.com/".Length);
        }
        else if (input.StartsWith("github.com/", StringComparison.OrdinalIgnoreCase))
        {
            input = input.Substring("github.com/".Length);
        }

        // Remove trailing slashes or .git suffix
        input = input.TrimEnd('/');
        if (input.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
        {
            input = input.Substring(0, input.Length - 4);
        }

        var parts = input.Split('/');
        if (parts.Length < 2)
            throw new ArgumentException($"Invalid repository format: {repoString}. Expected 'owner/repo' or full GitHub URL");

        return (parts[0].Trim(), parts[1].Trim());
    }

    private PullRequestState MapPrState(string? state, DateTime? mergedAt)
    {
        if (mergedAt.HasValue)
            return PullRequestState.Merged;
        
        return state?.ToLower() switch
        {
            "open" => PullRequestState.Open,
            "closed" => PullRequestState.Closed,
            _ => PullRequestState.Open
        };
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    #region GitHub API Response Models

    private class GitHubCommit
    {
        public string? Sha { get; set; }
        public GitHubCommitDetail? Commit { get; set; }
        public GitHubUser? Author { get; set; }
        public GitHubCommitStats? Stats { get; set; }
        public List<object>? Files { get; set; }
    }

    private class GitHubCommitDetail
    {
        public string? Message { get; set; }
        public GitHubAuthor? Author { get; set; }
    }

    private class GitHubAuthor
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime Date { get; set; }
    }

    private class GitHubCommitStats
    {
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public int Total { get; set; }
    }

    private class GitHubUser
    {
        public string? Login { get; set; }
    }

    private class GitHubPullRequest
    {
        public int Number { get; set; }
        public string? Title { get; set; }
        public string? State { get; set; }
        public GitHubUser? User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? MergedAt { get; set; }
        public int? Comments { get; set; }
        public int? ReviewComments { get; set; }
    }

    #endregion
}

#region Exceptions

public class GitHubAuthException : Exception
{
    public GitHubAuthException(string message) : base(message) { }
}

public class GitHubApiException : Exception
{
    public GitHubApiException(string message) : base(message) { }
}

public class GitHubRateLimitException : Exception
{
    public GitHubRateLimitException(string message) : base(message) { }
}

#endregion
