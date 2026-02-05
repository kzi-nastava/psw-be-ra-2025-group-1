using Explorer.ProjectAutopsy.API.Dtos;
using Explorer.ProjectAutopsy.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.ProjectAutopsy;

namespace Explorer.ProjectAutopsy.Tests.Integration;

[Collection("Sequential")]
public class AutopsyProjectTests : BaseProjectAutopsyIntegrationTest
{
    public AutopsyProjectTests(ProjectAutopsyTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_project()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new CreateAutopsyProjectDto
        {
            Name = "Test Project",
            Description = "Test Description",
            GitHubRepoUrl = "https://github.com/test/repo",
            JiraProjectKey = "TEST"
        };

        // Act
        var result = ((ObjectResult)controller.CreateProject(dto).Result)?.Value as AutopsyProjectDto;

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Project");
        result.Description.ShouldBe("Test Description");
        result.GitHubRepoUrl.ShouldBe("https://github.com/test/repo");
    }

    [Fact]
    public void Gets_all_projects()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        // Create test project first
        var dto = new CreateAutopsyProjectDto
        {
            Name = "Test Project 2",
            Description = "Test Description 2"
        };
        controller.CreateProject(dto);

        // Act
        var result = ((ObjectResult)controller.GetProjects().Result)?.Value as List<AutopsyProjectDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Gets_project_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        // Create test project
        var createDto = new CreateAutopsyProjectDto
        {
            Name = "Test Project 3",
            Description = "Test Description 3"
        };
        var created = ((ObjectResult)controller.CreateProject(createDto).Result)?.Value as AutopsyProjectDto;

        // Act
        var result = ((ObjectResult)controller.GetProject(created.Id).Result)?.Value as AutopsyProjectDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe("Test Project 3");
    }

    private static ProjectAutopsyController CreateController(IServiceScope scope)
    {
        return new ProjectAutopsyController(
            scope.ServiceProvider.GetRequiredService<IAutopsyProjectService>(),
            scope.ServiceProvider.GetRequiredService<IRiskAnalysisService>());
    }
}
