using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Tourist;

[Collection("Sequential")]
public class ProblemQueryTests : BaseStakeholdersIntegrationTest
{
    public ProblemQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<ProblemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThanOrEqualTo(7); // At least 7 test problems from seed data
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(7);
    }

    [Fact]
    public void Retrieves_my_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetMyProblems(0, 0).Result)?.Value as PagedResult<ProblemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.All(p => p.CreatorId == -21).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_problem_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();
        
        // Create a new problem for this test
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,  // This will be overridden by the service to match tour creator
            Priority = 3,
            Description = "Test problem for retrieval",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });

        // Act
        var result = ((ObjectResult)controller.GetById(newProblem.Id).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newProblem.Id);
        result.TourId.ShouldBe(-1);
        result.CreatorId.ShouldBe(-21);
        // AuthorId is set by the service based on the tour creator, not the input DTO
        result.AuthorId.ShouldBe(newProblem.AuthorId);  // Verify it matches what was created
        result.Status.ShouldBe(ProblemStatus.Open);
    }

    [Fact]
    public void Retrieves_resolved_problem_with_comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();
        
        // Create a problem and resolve it
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,
            Priority = 3,
            Description = "Test problem to resolve",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });
        
        // Resolve it
        service.ChangeProblemStatus(newProblem.Id, -21, ProblemStatus.ResolvedByTourist, "Issue was fixed quickly");

        // Act
        var result = ((ObjectResult)controller.GetById(newProblem.Id).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newProblem.Id);
        result.Status.ShouldBe(ProblemStatus.ResolvedByTourist);
        result.TouristComment.ShouldBe("Issue was fixed quickly");
        result.ResolvedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Retrieves_problem_with_admin_deadline()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();
        
        // Create a problem
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,
            Priority = 3,
            Description = "Test problem for deadline",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });
        
        // Set admin deadline
        var deadline = DateTime.UtcNow.AddDays(7);
        service.SetAdminDeadline(newProblem.Id, deadline);

        // Act
        var result = ((ObjectResult)controller.GetById(newProblem.Id).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newProblem.Id);
        result.AdminDeadline.ShouldNotBeNull();
        result.AdminDeadline.Value.Date.ShouldBe(deadline.Date);
    }

    [Fact]
    public void Late_flag_is_set_for_overdue_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<Stakeholders.Infrastructure.Database.StakeholdersContext>();
        
        // Create a problem with a very old creation time (more than 5 days ago)
        var oldProblem = new Stakeholders.Core.Domain.Problem(
            priority: 3,
            description: "Very old problem for overdue test",
            category: Stakeholders.Core.Domain.ProblemCategory.Other,
            tourId: -1,
            creatorId: -21,
            authorId: -11
        );
        
        // Add it directly to database so we can control the creation time
        dbContext.Problems.Add(oldProblem);
        dbContext.SaveChanges();
        
        // Get the ID before clearing tracker
        var problemId = oldProblem.Id;
        
        // Now manually set the CreationTime to be older than 5 days using raw SQL
        var oldDate = DateTime.UtcNow.AddDays(-10);
        dbContext.Database.ExecuteSqlRaw(
            $"UPDATE stakeholders.\"Problems\" SET \"CreationTime\" = '{oldDate:yyyy-MM-dd HH:mm:ss}' WHERE \"Id\" = {problemId}");
        
        // IMPORTANT: Clear the EF tracker so it doesn't use cached data
        dbContext.ChangeTracker.Clear();
        
        var controller = CreateController(scope);

        // Act - This will fetch fresh data from database
        var result = ((ObjectResult)controller.GetById(problemId).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(problemId);
        result.Status.ShouldBe(ProblemStatus.Open);
        result.LateFlag.ShouldBeTrue(); // Problem is older than 5 days and still open
    }

    private static TouristProblemController CreateController(IServiceScope scope)
    {
        return new TouristProblemController(
            scope.ServiceProvider.GetRequiredService<IProblemService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Administration.ITourService>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("id", "-21"),
                        new Claim("personId", "-21"),
                        new Claim(ClaimTypes.Role, "tourist")
                    }, "test"))
                }
            }
        };
    }
}
