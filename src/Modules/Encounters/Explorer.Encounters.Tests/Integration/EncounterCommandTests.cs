using Explorer.API.Controllers;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class EncounterCommandTests : BaseEncountersIntegrationTest
{
    public EncounterCommandTests(EncountersTestFactory factory) : base(factory) { }

    private static EncounterController CreateController(IServiceScope scope, long touristId)
    {
        var controller = new EncounterController(
            scope.ServiceProvider.GetRequiredService<IEncounterService>(),
            scope.ServiceProvider.GetRequiredService<ITouristStatsService>()
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", touristId.ToString()),
                    new Claim("id", touristId.ToString()),
                    new Claim(ClaimTypes.Role, "tourist")
                }, "test"))
            }
        };

        return controller;
    }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1); //Admin
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();
        var newEntity = new EncounterCreateDto
        {
            Title = "Test Encounter",
            Description = "Test Description",
            Longitude = 20.5,
            Latitude = 44.8,
            Xp = 100,
            Type = "Social",
            RequiredPeopleCount = 1,
            Range = 50
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as EncounterDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe(newEntity.Title);

        // Assert - Database
        var storedEntity = dbContext.Encounters.FirstOrDefault(i => i.Title == newEntity.Title);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);
        var newEntity = new EncounterCreateDto
        {
            Title = "",
            Description = "Test Description",
            Longitude = 20.5,
            Latitude = 44.8,
            Xp = 100,
            Type = "Social",
            RequiredPeopleCount = -1
        };

        // Act
        var result = controller.Create(newEntity).Result as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Gets_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);

        // Act
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<EncounterDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Gets_active()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 2);

        // Act
        var result = ((ObjectResult)controller.GetActive().Result)?.Value as List<EncounterDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldAllBe(e => e.Status == "Active");
    }

    [Fact]
    public void Gets_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 2);

        // Act
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as EncounterDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
    }

    [Fact]
    public void Get_by_id_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);

        // Act
        var result = controller.GetById(-1000).Result as NotFoundObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();
        var updatedEntity = new EncounterCreateDto
        {
            Title = "Updated Encounter",
            Description = "Updated Description",
            Longitude = 21.0,
            Latitude = 45.0,
            Xp = 150,
            Type = "Location",
            HiddenLongitude = 21.5,
            HiddenLatitude = 45.5,
            ImagePath = "some/path",
            Hints = []
        };

        // Act
        var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as HiddenEncounterDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Title.ShouldBe(updatedEntity.Title);
        result.Description.ShouldBe(updatedEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.Encounters.FirstOrDefault(i => i.Title == "Updated Encounter");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);
        var updatedEntity = new EncounterCreateDto
        {
            Title = "Test",
            Description = "Test",
            Longitude = 20.0,
            Latitude = 44.0,
            Xp = 100,
            Type = "Social"
        };

        // Act
        var result = controller.Update(-1000, updatedEntity).Result as NotFoundObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Publishes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();

        // Act
        var result = controller.Publish(-1) as OkResult;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Encounters.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(Explorer.Encounters.Core.Domain.EncounterStatus.Active);
    }

    [Fact]
    public void Publish_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);

        // Act
        var result = controller.Publish(-1000) as NotFoundObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Archives()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();

        // Act
        var result = controller.Archive(-1) as OkResult;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Encounters.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(Explorer.Encounters.Core.Domain.EncounterStatus.Archived);
    }

    [Fact]
    public void Archive_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);

        // Act
        var result = controller.Archive(-1000) as NotFoundObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();

        // Act
        var result = controller.Delete(-3) as OkResult;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Encounters.FirstOrDefault(i => i.Id == -3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1);

        // Act
        var result = controller.Delete(-1000) as NotFoundObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }
}