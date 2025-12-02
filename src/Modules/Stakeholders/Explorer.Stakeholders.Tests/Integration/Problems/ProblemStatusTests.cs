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

        // Act
        var result = service.ChangeProblemStatus(-1, -21, ProblemStatus.ResolvedByTourist, "Issue was fixed");

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Status.ShouldBe(ProblemStatus.ResolvedByTourist);
        result.TouristComment.ShouldBe("Issue was fixed");
        result.ResolvedAt.ShouldNotBeNull();

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProblem = dbContext.Problems.Find(-1L);
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

        // Act
        var result = service.ChangeProblemStatus(-2, -22, ProblemStatus.Unresolved, "Still not fixed");

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProblemStatus.Unresolved);
        result.TouristComment.ShouldBe("Still not fixed");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProblem = dbContext.Problems.Find(-2L);
        storedProblem.Status.ShouldBe(Core.Domain.ProblemStatus.Unresolved);
    }

    [Fact]
    public void Change_status_fails_when_not_problem_creator()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Act & Assert - Tourist -23 trying to change status of problem -1 (created by -21)
        Should.Throw<UnauthorizedAccessException>(() => 
            service.ChangeProblemStatus(-1, -23, ProblemStatus.ResolvedByTourist, "I'm not the creator"));
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

        // Act - Tourist accessing their own problem
        var result = service.Get(-1, -21);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.CreatorId.ShouldBe(-21);
    }

    [Fact]
    public void Get_problem_fails_when_not_participant()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();

        // Act & Assert 
        Should.Throw<UnauthorizedAccessException>(() => service.Get(-1, -23));
    }

    [Fact]
    public void Admin_sets_deadline_for_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();
        var deadline = DateTime.UtcNow.AddDays(7);

        // Act
        var result = service.SetAdminDeadline(-1, deadline);

        // Assert - Response
        result.ShouldNotBeNull();
        result.AdminDeadline.ShouldNotBeNull();
        result.AdminDeadline.Value.Date.ShouldBe(deadline.Date);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProblem = dbContext.Problems.Find(-1L);
        storedProblem.ShouldNotBeNull();
        storedProblem.AdminDeadline.ShouldNotBeNull();
    }

    [Fact]
    public void Set_deadline_fails_with_past_date()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProblemService>();
        var pastDeadline = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.SetAdminDeadline(-1, pastDeadline));
    }
}
