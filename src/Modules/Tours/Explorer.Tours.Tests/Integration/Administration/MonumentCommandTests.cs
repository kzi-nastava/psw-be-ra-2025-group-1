using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MonumentCommandTests : BaseToursIntegrationTest
{
    public MonumentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new CreateMonumentDto
        {
            Name = "TestMonument" + Guid.NewGuid().ToString().Substring(0, 8),
            Description = "TestDescription",
            Longitude = 20.5,
            Latitude = 44.8
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);

        // Assert - Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var newEntity = new CreateMonumentDto
        {
            Name = "",
            Description = "TestDescription",
            Longitude = 20.5,
            Latitude = 44.8
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
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<MonumentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Gets_all_paged()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAllPaged(0, 10).Result)?.Value as PagedResult<MonumentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Gets_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as MonumentDto;

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
        Should.Throw<NotFoundException>(() => controller.GetById(-1000));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new MonumentDto
        {
            Id = -1,
            Name = "UpdatedMonument",
            Description = "UpdatedDescription",
            CreationYear = 2020,
            Status = "Active",
            Longitude = 21.0,
            Latitude = 45.0
        };

        // Act
        var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Name == "UpdatedMonument");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new MonumentDto
        {
            Id = -1000,
            Name = "Test",
            Description = "Test",
            CreationYear = 2020,
            Status = "Active",
            Longitude = 20.0,
            Latitude = 44.0
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // First create a monument to delete
        var monumentToDelete = new CreateMonumentDto
        {
            Name = "Monument to Delete " + Guid.NewGuid().ToString().Substring(0, 8),
            Description = "This will be deleted",
            Longitude = 20.5,
            Latitude = 44.8
        };
        var created = ((ObjectResult)controller.Create(monumentToDelete).Result)?.Value as MonumentDto;

        // Act
        var result = (OkResult)controller.Delete(created.Id);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Id == created.Id);
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

    private static MonumentController CreateController(IServiceScope scope)
    {
        return new MonumentController(scope.ServiceProvider.GetRequiredService<IMonumentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}