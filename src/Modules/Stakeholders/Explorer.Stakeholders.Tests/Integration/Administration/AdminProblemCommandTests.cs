using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminProblemCommandTests : BaseStakeholdersIntegrationTest
{
    public AdminProblemCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Admin_sets_deadline_for_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var deadline = DateTime.UtcNow.AddDays(7);
        var dto = new SetDeadlineDto { Deadline = deadline };

        // Act
        var result = ((ObjectResult)controller.SetDeadline(-1, dto).Result)?.Value as ProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.AdminDeadline.ShouldNotBeNull();
        result.AdminDeadline.Value.Date.ShouldBe(deadline.Date);

        // Assert - Database
        var storedEntity = dbContext.Problems.FirstOrDefault(p => p.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.AdminDeadline.ShouldNotBeNull();
    }

    [Fact]
    public void Admin_closes_problem_with_comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var dto = new CloseProblemDto { Comment = "Closing due to policy violation" };

        // Act
        var result = ((ObjectResult)controller.CloseProblem(-1, dto).Result)?.Value as ProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Status.ShouldBe(ProblemStatus.Unresolved);
        result.TouristComment.ShouldBe("Closing due to policy violation");
        result.ResolvedAt.ShouldNotBeNull();

        // Assert - Database
        var storedEntity = dbContext.Problems.FirstOrDefault(p => p.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(Core.Domain.ProblemStatus.Unresolved);
    }

    [Fact]
    public void Admin_closes_problem_without_comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var result = ((ObjectResult)controller.CloseProblem(-2, null).Result)?.Value as ProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-2);
        result.Status.ShouldBe(ProblemStatus.Unresolved);
        result.TouristComment.ShouldBe("Closed by administrator");

        // Assert - Database
        var storedEntity = dbContext.Problems.FirstOrDefault(p => p.Id == -2);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(Core.Domain.ProblemStatus.Unresolved);
    }

    [Fact]
    public void Admin_can_close_already_resolved_problem()
    {
        // Arrange - Problem -4 is already resolved by tourist
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var dto = new CloseProblemDto { Comment = "Admin override" };

        // Act
        var result = ((ObjectResult)controller.CloseProblem(-4, dto).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProblemStatus.Unresolved);
        result.TouristComment.ShouldBe("Admin override");
    }

    [Fact]
    public void Gets_overdue_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetOverdueProblems(5).Result)?.Value as List<ProblemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        // Problem -6 is from 2024-01-01, so it should be overdue
        result.ShouldContain(p => p.Id == -6);
    }

    [Fact]
    public void Gets_all_problems_as_admin()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAllProblems(0, 0).Result)?.Value as PagedResult<ProblemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(7);
        result.TotalCount.ShouldBe(7);
    }

    [Fact]
    public void Gets_problem_by_id_as_admin()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetProblemById(-1).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.TourId.ShouldBe(-1);
    }

    private static AdminProblemController CreateController(IServiceScope scope)
    {
        return new AdminProblemController(scope.ServiceProvider.GetRequiredService<IProblemService>())
        {
            ControllerContext = BuildContext("-1") // Admin user
        };
    }
}
