using Explorer.ProjectAutopsy.Core.Services;

namespace Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;

/// <summary>
/// Service interface for fetching GitHub data.
/// Abstracts external GitHub API calls from the Core layer.
/// </summary>
public interface IGitHubDataService
{
    /// <summary>
    /// Fetches commits from a GitHub repository within the specified date range.
    /// </summary>
    /// <param name="ownerRepo">Repository in format "owner/repo"</param>
    /// <param name="since">Start date for commit history</param>
    /// <param name="until">End date for commit history</param>
    /// <returns>List of commit data</returns>
    Task<List<CommitData>> FetchCommitsAsync(string ownerRepo, DateTime since, DateTime until);

    /// <summary>
    /// Fetches pull requests from a GitHub repository within the specified date range.
    /// </summary>
    /// <param name="ownerRepo">Repository in format "owner/repo"</param>
    /// <param name="since">Start date for PR history</param>
    /// <param name="until">End date for PR history</param>
    /// <returns>List of pull request data</returns>
    Task<List<PullRequestData>> FetchPullRequestsAsync(string ownerRepo, DateTime since, DateTime until);

    /// <summary>
    /// Validates that the GitHub connection is working.
    /// </summary>
    /// <returns>True if connection is valid, false otherwise</returns>
    Task<bool> ValidateConnectionAsync();
}
