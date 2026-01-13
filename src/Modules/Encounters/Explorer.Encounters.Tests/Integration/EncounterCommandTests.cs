using Explorer.API.Controllers;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class EncounterCommandTests : BaseEncountersIntegrationTest
{
    public EncounterCommandTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();
        var newEntity = new EncounterCreateDto
        {
            Title = "Test Encounter",
            Description = "Test Description",
            Longitude = 20.5,
            Latitude = 44.8,
            Xp = 100,
            Type = "Social"
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
        var controller = CreateController(scope);
        var newEntity = new EncounterCreateDto
        {
            Title = "",
            Description = "Test Description",
            Longitude = 20.5,
            Latitude = 44.8,
            Xp = 100,
            Type = "Social"
        };

        // Act & Assert
        Should.Throw<EntityValidationException>(() => controller.Create(newEntity));
    }

    [Fact]
    public void Gets_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

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
        var controller = CreateController(scope);

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
        var controller = CreateController(scope);

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
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => controller.GetById(-1000));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();
        var updatedEntity = new EncounterCreateDto
        {
            Title = "Updated Encounter",
            Description = "Updated Description",
            Longitude = 21.0,
            Latitude = 45.0,
            Xp = 150,
            Type = "Location"
        };

        // Act
        var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as EncounterDto;

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
        var controller = CreateController(scope);
        var updatedEntity = new EncounterCreateDto
        {
            Title = "Test",
            Description = "Test",
            Longitude = 20.0,
            Latitude = 44.0,
            Xp = 100,
            Type = "Social"
        };

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public void Publishes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();

        // Act
        var result = (OkResult)controller.Publish(-1);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Encounters.FirstOrDefault(i => i.Id == -2);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(Explorer.Encounters.Core.Domain.EncounterStatus.Active);
    }

    [Fact]
    public void Publish_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => controller.Publish(-1000));
    }

    [Fact]
    public void Archives()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();

        // Act
        var result = (OkResult)controller.Archive(-1);

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
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => controller.Archive(-1000));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncounterContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

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
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => controller.Delete(-1000));
    }

    private static EncounterController CreateController(IServiceScope scope)
    {
        return new EncounterController(scope.ServiceProvider.GetRequiredService<IEncounterService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}