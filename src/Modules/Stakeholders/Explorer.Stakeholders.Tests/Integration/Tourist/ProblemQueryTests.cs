using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        result.Results.Count.ShouldBe(7); // Updated: now we have 7 test problems
        result.TotalCount.ShouldBe(7);
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
        result.Results.Count.ShouldBe(4); // Tourist -21 has problems -1, -4, -5, -7
    }

    [Fact]
    public void Retrieves_problem_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.TourId.ShouldBe(-1);
        result.CreatorId.ShouldBe(-21);
        result.AuthorId.ShouldBe(-11);
        result.Status.ShouldBe(ProblemStatus.Open);
    }

    [Fact]
    public void Retrieves_resolved_problem_with_comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetById(-4).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-4);
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

        // Act - Problem -7 belongs to user -21 and has an admin deadline
        var result = ((ObjectResult)controller.GetById(-7).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-7);
        result.AdminDeadline.ShouldNotBeNull();
    }

    [Fact]
    public void Late_flag_is_set_for_overdue_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Problem -1 is old (from 2024-01-15) and belongs to user -21
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as ProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.LateFlag.ShouldBeTrue(); // Problem is older than 5 days and still open
    }

    private static TouristProblemController CreateController(IServiceScope scope)
    {
        return new TouristProblemController(scope.ServiceProvider.GetRequiredService<IProblemService>())
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
