using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using Explorer.ProjectAutopsy.Core.Services;

namespace Explorer.ProjectAutopsy.Infrastructure.ExternalClients;

/// <summary>
/// Implementation of IGitHubDataService that uses GitHubClient to fetch data.
/// Bridges the Core layer interface with the Infrastructure GitHub client.
/// </summary>
public class GitHubDataService : IGitHubDataService
{
    private readonly GitHubClient _client;

    public GitHubDataService(GitHubClient client)
    {
        _client = client;
    }

    public async Task<List<CommitData>> FetchCommitsAsync(string ownerRepo, DateTime since, DateTime until)
    {
        var (owner, repo) = GitHubClient.ParseRepoString(ownerRepo);

        // Fetch all commits since the start date
        var commits = await _client.FetchCommitsAsync(owner, repo, since, maxResults: 1000);

        // Filter to only include commits within the date range
        return commits
            .Where(c => c.CommittedAt >= since && c.CommittedAt <= until)
            .ToList();
    }

    public async Task<List<PullRequestData>> FetchPullRequestsAsync(string ownerRepo, DateTime since, DateTime until)
    {
        var (owner, repo) = GitHubClient.ParseRepoString(ownerRepo);

        // Fetch all PRs since the start date
        var pullRequests = await _client.FetchPullRequestsAsync(owner, repo, since, maxResults: 1000);

        // Filter to only include PRs within the date range
        return pullRequests
            .Where(pr => pr.CreatedAt >= since && pr.CreatedAt <= until)
            .ToList();
    }

    public async Task<bool> ValidateConnectionAsync()
    {
        return await _client.ValidateConnectionAsync();
    }
}
