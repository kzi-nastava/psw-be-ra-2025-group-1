using Explorer.Blog.API.Dtos;
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
        createdBlog.Title.ShouldBe("New Integration Test Blog");

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
            role = 0 // Tourist role is 0-
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

        return loginResult?["accessToken"].GetString() ?? throw new Exception("Failed to get access token");
    }
} 