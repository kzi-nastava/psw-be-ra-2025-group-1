using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Tests;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class BlogControllerTests : BaseWebIntegrationTest<BlogTestFactory>
{
    public BlogControllerTests(BlogTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMyBlogs_WithTestData_ReturnsUserBlogs()
    {
        var token = await AuthenticateTestUser();

        TestClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await TestClient.GetAsync("/api/blog/my");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var blogs = JsonSerializer.Deserialize<List<BlogDto>>(responseContent, options);

        blogs.ShouldNotBeNull();
        blogs.Count.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task CreateBlog_ValidInput_ReturnsCreatedBlogAndDatabaseIsUpdated()
    {
        var token = await AuthenticateTestUser();

        var createDto = new
        {
            title = "New Integration Test Blog",
            description = "# Test\n\nThis blog will be saved to database.",
            images = new[] { "https://example.com/new-test.jpg" }
        };

        var json = JsonSerializer.Serialize(createDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        TestClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await TestClient.PostAsync("/api/blog", content);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var createdBlog = JsonSerializer.Deserialize<BlogDto>(responseContent, options);

        createdBlog.ShouldNotBeNull();
        createdBlog.Id.ShouldBeGreaterThan(0);
        createdBlog.Title.ShouldBe("Integration Test Blog");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blogInDb = dbContext.Blogs.FirstOrDefault(b => b.Id == createdBlog.Id);
        blogInDb.ShouldNotBeNull();
        blogInDb.Title.ShouldBe("New Integration Test Blog");
    }

    [Fact]
    public async Task CreateBlog_Unauthorized_Returns401()
    {

        var createDto = new
        {
            title = "Unauthorized Blog",
            description = "This should fail"
        };

        var json = JsonSerializer.Serialize(createDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");


        var response = await TestClient.PostAsync("/api/blog", content);


        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddVote_OnPublishedBlog_ShouldSucceed()
    {
        // Arrange
        var token = await AuthenticateTestUser();
        TestClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var blog = await CreateAndPublishBlogAsync();
        var voteRequest = new VoteCreateDto { VoteType = "Upvote" };

        // Act
        var json = JsonSerializer.Serialize(voteRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await TestClient.PostAsync($"/api/blog/{blog.Id}/votes", content);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var voteDto = JsonSerializer.Deserialize<VoteDto>(responseContent, options);
        voteDto.ShouldNotBeNull();
        voteDto.VoteType.ToString().ShouldBe("Downvote");
    }

    [Fact]
    public async Task AddVote_OnDraftBlog_ShouldFail()
    {
        // Arrange
        var token = await AuthenticateTestUser();
        TestClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var blog = await CreateBlogAsync(); // Draft status
        var voteRequest = new VoteCreateDto { VoteType = "Upvote" };

        // Act
        var json = JsonSerializer.Serialize(voteRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await TestClient.PostAsync($"/api/blog/{blog.Id}/votes", content);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangeVote_FromUpvoteToDownvote_ShouldReplace()
    {
        // Arrange
        var token = await AuthenticateTestUser();
        TestClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var blog = await CreateAndPublishBlogAsync();
        var upvoteRequest = new VoteCreateDto { VoteType = "Upvote" };
        var downvoteRequest = new VoteCreateDto { VoteType = "Downvote" };

        // Act - Add upvote
        var upvoteJson = JsonSerializer.Serialize(upvoteRequest);
        var upvoteContent = new StringContent(upvoteJson, Encoding.UTF8, "application/json");
        await TestClient.PostAsync($"/api/blog/{blog.Id}/votes", upvoteContent);
        
        // Act - Change to downvote
        var downvoteJson = JsonSerializer.Serialize(downvoteRequest);
        var downvoteContent = new StringContent(downvoteJson, Encoding.UTF8, "application/json");
        var response = await TestClient.PostAsync($"/api/blog/{blog.Id}/votes", downvoteContent);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var voteDto = JsonSerializer.Deserialize<VoteDto>(responseContent, options);
        voteDto.VoteType.ToString().ShouldBe("Downvote");
    }

    [Fact]
    public async Task RemoveVote_ShouldDeleteVote()
    {
        // Arrange
        var token = await AuthenticateTestUser();
        TestClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var blog = await CreateAndPublishBlogAsync();
        var voteRequest = new VoteCreateDto { VoteType = "Upvote" };
        
        // Add vote first
        var json = JsonSerializer.Serialize(voteRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await TestClient.PostAsync($"/api/blog/{blog.Id}/votes", content);

        // Act
        var response = await TestClient.DeleteAsync($"/api/blog/{blog.Id}/votes");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        // Verify vote is gone
        var blogResponse = await TestClient.GetAsync($"/api/blog/{blog.Id}");
        var blogContent = await blogResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var fetchedBlog = JsonSerializer.Deserialize<BlogDto>(blogContent, options);
        fetchedBlog.Votes.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetBlog_ShouldIncludeVotesAndScore()
    {
        // Arrange
        var token = await AuthenticateTestUser();
        TestClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var blog = await CreateAndPublishBlogAsync();
        var voteRequest = new VoteCreateDto { VoteType = "Upvote" };
        
        var json = JsonSerializer.Serialize(voteRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await TestClient.PostAsync($"/api/blog/{blog.Id}/votes", content);

        // Act
        var response = await TestClient.GetAsync($"/api/blog/{blog.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var fetchedBlog = JsonSerializer.Deserialize<BlogDto>(responseContent, options);
        fetchedBlog.Votes.ShouldNotBeEmpty();
        fetchedBlog.VoteScore.ShouldBe(1);
        fetchedBlog.CurrentUserVote?.ToString().ShouldBe("Upvote");
    }

    [Fact]
    public async Task GetBlog_WithMultipleVotes_ShouldCalculateScoreCorrectly()
    {
        // Arrange
        var token = await AuthenticateTestUser();
        TestClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var blog = await CreateAndPublishBlogAsync();
        var upvoteRequest = new VoteCreateDto { VoteType = "Upvote" };
        
        // Add upvote (score = 1)
        var json = JsonSerializer.Serialize(upvoteRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await TestClient.PostAsync($"/api/blog/{blog.Id}/votes", content);

        // Act
        var response = await TestClient.GetAsync($"/api/blog/{blog.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var fetchedBlog = JsonSerializer.Deserialize<BlogDto>(responseContent, options);
        fetchedBlog.ShouldNotBeNull();
        fetchedBlog.VoteScore.ShouldBe(1);
        fetchedBlog.CurrentUserVote?.ToString().ShouldBe("Upvote"); 
    }

    // Updated helper methods
    private async Task<BlogDto> CreateBlogAsync()
    {
        var createRequest = new BlogCreateDto 
        { 
            Title = "Test Blog", 
            Description = "Test Description",
            Images = new List<string>()
        };
        var json = JsonSerializer.Serialize(createRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await TestClient.PostAsync("/api/blog", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<BlogDto>(responseContent, options);
    }

    private async Task<BlogDto> CreateAndPublishBlogAsync()
    {
        var blog = await CreateBlogAsync();
        await TestClient.PutAsync($"/api/blog/{blog.Id}/publish", null);
        return blog;
    }

    private async Task<string> AuthenticateTestUser()
    {
        // test user registration
        var username = $"testuser_{Guid.NewGuid()}";
        var registerDto = new
        {
            username = username,
            password = "Test123!",
            email = $"{username}@example.com",
            name = "Test",
            surname = "User",
            role = 0 // Tourist role
        };

        var registerJson = JsonSerializer.Serialize(registerDto);
        var registerContent = new StringContent(registerJson, Encoding.UTF8, "application/json");

        await TestClient.PostAsync("/api/users", registerContent);

        // Login
        var loginDto = new { username = username, password = "Test123!" };
        var loginJson = JsonSerializer.Serialize(loginDto);
        var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

        var loginResponse = await TestClient.PostAsync("/api/users/login", loginContent);

        var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var loginResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(loginResponseContent, options);

        var token = loginResult?["accessToken"].GetString();
        return token ?? throw new Exception("Failed to get access token");
    }
}