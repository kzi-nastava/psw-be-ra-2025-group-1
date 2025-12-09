using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Tourist;

[Collection("Sequential")]
public class ProblemCommandTests : BaseStakeholdersIntegrationTest
{
    public ProblemCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var newEntity = new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            Priority = 3,
            Description = "I was left alone",
            Category = ProblemCategory.Safety
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as ProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Description.ShouldBe(newEntity.Description);
        result.Priority.ShouldBe(newEntity.Priority);
        result.Status.ShouldBe(ProblemStatus.Open);
        result.CreationTime.ShouldNotBe(default);

        // Assert - Database
        var storedEntity = dbContext.Problems.FirstOrDefault(i => i.Description == newEntity.Description);
        storedEntity.ShouldNotBeNull();
        storedEntity.Priority.ShouldBe(result.Priority);
        storedEntity.Status.ShouldBe(Core.Domain.ProblemStatus.Open);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new ProblemDto
        {
            TourId = -1,
            CreatorId = -21,
            Priority = 6,
            Description = "Test problem"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var updatedEntity = new ProblemDto
        {
            Id = -1,
            TourId = -1,
            CreatorId = -21,
            Priority = 4,
            Description = "Dangerous cliff",
            Category = ProblemCategory.Safety
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as ProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Description.ShouldBe(updatedEntity.Description);
        result.Priority.ShouldBe(updatedEntity.Priority);

        // Assert - Database
        var storedEntity = dbContext.Problems.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new ProblemDto
        {
            Id = -1000,
            TourId = -1,
            CreatorId = -21,
            Priority = 2,
            Description = "Test"
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Problems.FirstOrDefault(i => i.Id == -3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    [Fact]
    public void Changes_status_to_resolved()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var statusDto = new ChangeProblemStatusDto
        {
            Status = ProblemStatus.ResolvedByTourist,
            Comment = "The issue has been fixed"
        };

        // Act
        var result = ((ObjectResult)controller.ChangeStatus(-1, statusDto).Result)?.Value as ProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Status.ShouldBe(ProblemStatus.ResolvedByTourist);
        result.TouristComment.ShouldBe("The issue has been fixed");
        result.ResolvedAt.ShouldNotBeNull();

        // Assert - Database
        var storedEntity = dbContext.Problems.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(Core.Domain.ProblemStatus.ResolvedByTourist);
        storedEntity.TouristComment.ShouldBe("The issue has been fixed");
    }

    [Fact]
    public void Changes_status_to_unresolved()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var statusDto = new ChangeProblemStatusDto
        {
            Status = ProblemStatus.Unresolved,
            Comment = "Still waiting for response"
        };

        // Act - Using problem -7 which is open and belongs to user -21
        var result = ((ObjectResult)controller.ChangeStatus(-7, statusDto).Result)?.Value as ProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-7);
        result.Status.ShouldBe(ProblemStatus.Unresolved);
        result.TouristComment.ShouldBe("Still waiting for response");
        result.ResolvedAt.ShouldNotBeNull();

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedEntity = dbContext.Problems.FirstOrDefault(i => i.Id == -7);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(Core.Domain.ProblemStatus.Unresolved);
    }

    [Fact]
    public void Change_status_fails_for_unauthorized_user()
    {
        // Arrange - User -22 trying to change problem created by user -21
        using var scope = Factory.Services.CreateScope();
        var controller = new TouristProblemController(scope.ServiceProvider.GetRequiredService<IProblemService>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("id", "-22"),
                        new Claim("personId", "-22"),
                        new Claim(ClaimTypes.Role, "tourist")
                    }, "test"))
                }
            }
        };
        var statusDto = new ChangeProblemStatusDto
        {
            Status = ProblemStatus.ResolvedByTourist,
            Comment = "Trying to change someone else's problem"
        };

        // Act
        var result = controller.ChangeStatus(-1, statusDto).Result;

        // Assert
        result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Change_status_fails_for_non_open_problem()
    {
        // Arrange - Problem -4 is already resolved
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var statusDto = new ChangeProblemStatusDto
        {
            Status = ProblemStatus.ResolvedByTourist,
            Comment = "Trying to change again"
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.ChangeStatus(-4, statusDto));
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
