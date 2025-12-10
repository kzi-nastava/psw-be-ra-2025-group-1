using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Problems;

[Collection("Sequential")]
public class ProblemStatusTests : BaseStakeholdersIntegrationTest
{
    public ProblemStatusTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Tourist_changes_problem_status_to_resolved()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Create a new problem for this test to avoid interference with other tests
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,
            Priority = 3,
            Description = "Test problem for status change",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });

        // Act
        var result = service.ChangeProblemStatus(newProblem.Id, -21, ProblemStatus.ResolvedByTourist, "Issue was fixed");

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newProblem.Id);
        result.Status.ShouldBe(ProblemStatus.ResolvedByTourist);
        result.TouristComment.ShouldBe("Issue was fixed");
        result.ResolvedAt.ShouldNotBeNull();

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProblem = dbContext.Problems.Find(newProblem.Id);
        storedProblem.ShouldNotBeNull();
        storedProblem.Status.ShouldBe(Core.Domain.ProblemStatus.ResolvedByTourist);
        storedProblem.TouristComment.ShouldBe("Issue was fixed");
    }

    [Fact]
    public void Tourist_changes_problem_status_to_unresolved()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Create a new problem for this test - use TourId = -1 to avoid cross-module issues
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,  // Changed from -2 to -1 to use existing tour
            CreatorId = -22,
            AuthorId = -11,  // Updated to match tour -1's creator
            Priority = 5,
            Description = "Test problem for unresolved status",
            Category = ProblemCategory.Maintenance,
            CreationTime = DateTime.UtcNow
        });

        // Act
        var result = service.ChangeProblemStatus(newProblem.Id, -22, ProblemStatus.Unresolved, "Still not fixed");

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProblemStatus.Unresolved);
        result.TouristComment.ShouldBe("Still not fixed");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProblem = dbContext.Problems.Find(newProblem.Id);
        storedProblem.Status.ShouldBe(Core.Domain.ProblemStatus.Unresolved);
    }

    [Fact]
    public void Change_status_fails_when_not_problem_creator()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Create a problem owned by tourist -21
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,
            Priority = 3,
            Description = "Test problem for unauthorized access",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });

        // Act & Assert - Tourist -23 trying to change status of problem created by -21
        Should.Throw<UnauthorizedAccessException>(() => 
            service.ChangeProblemStatus(newProblem.Id, -23, ProblemStatus.ResolvedByTourist, "I'm not the creator"));
    }

    [Fact]
    public void Change_status_fails_for_nonexistent_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Act & Assert
        Should.Throw<NotFoundException>(() => 
            service.ChangeProblemStatus(-999, -21, ProblemStatus.ResolvedByTourist, "Comment"));
    }

    [Fact]
    public void Get_problems_by_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Act
        var result = service.GetByAuthor(-11, 1, 10);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.ShouldAllBe(p => p.AuthorId == -11);
    }

    [Fact]
    public void Get_problem_with_access_check()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Create a problem for this test
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,
            Priority = 3,
            Description = "Test problem for access check",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });

        // Act - Tourist accessing their own problem
        var result = service.Get(newProblem.Id, -21);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newProblem.Id);
        result.CreatorId.ShouldBe(-21);
    }

    [Fact]
    public void Get_problem_fails_when_not_participant()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Create a problem owned by tourist -21
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,
            Priority = 3,
            Description = "Test problem for unauthorized access",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });

        // Act & Assert - Tourist -23 trying to access problem created by -21
        Should.Throw<UnauthorizedAccessException>(() => service.Get(newProblem.Id, -23));
    }

    [Fact]
    public void Admin_sets_deadline_for_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Create a problem for this test
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

        var deadline = DateTime.UtcNow.AddDays(7);

        // Act
        var result = service.SetAdminDeadline(newProblem.Id, deadline);

        // Assert - Response
        result.ShouldNotBeNull();
        result.AdminDeadline.ShouldNotBeNull();
        result.AdminDeadline.Value.Date.ShouldBe(deadline.Date);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProblem = dbContext.Problems.Find(newProblem.Id);
        storedProblem.ShouldNotBeNull();
        storedProblem.AdminDeadline.ShouldNotBeNull();
    }

    [Fact]
    public void Set_deadline_fails_with_past_date()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Create a problem for this test
        var newProblem = service.Create(new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            AuthorId = -11,
            Priority = 3,
            Description = "Test problem for past deadline",
            Category = ProblemCategory.Other,
            CreationTime = DateTime.UtcNow
        });

        var pastDeadline = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.SetAdminDeadline(newProblem.Id, pastDeadline));
    }
}
