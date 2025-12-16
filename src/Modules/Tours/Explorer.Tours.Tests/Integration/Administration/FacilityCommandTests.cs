using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class FacilityCommandTests : BaseToursIntegrationTest
{
    public FacilityCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new FacilityDto
        {
            Name = "New Test Facility " + Guid.NewGuid().ToString().Substring(0, 8),
            Latitude = 40.779437,
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.Parking
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as FacilityDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Latitude.ShouldBe(newEntity.Latitude);
        result.Longitude.ShouldBe(newEntity.Longitude);
        result.Category.ShouldBe(newEntity.Category);
        
        // Assert - Database
        var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new FacilityDto
        {
            Name = "",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.WC
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Create_fails_invalid_latitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new FacilityDto
        {
            Name = "Test Facility",
            Latitude = 100.0, // Invalid: exceeds valid range
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.Restaurant
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_invalid_longitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new FacilityDto
        {
            Name = "Test Facility",
            Latitude = 40.779437,
            Longitude = 200.0, // Invalid: exceeds valid range
            Category = API.Dtos.FacilityCategory.Restaurant
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_duplicate_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        var uniqueName = "Unique Test Facility " + Guid.NewGuid().ToString().Substring(0, 8);
        
        // First, create a facility with a unique name
        var originalEntity = new FacilityDto
        {
            Name = uniqueName,
            Latitude = 40.779437,
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.WC
        };
        controller.Create(originalEntity);
        
        // Now try to create a duplicate
        var duplicateEntity = new FacilityDto
        {
            Name = uniqueName, // Same name
            Latitude = 41.0,
            Longitude = -74.0,
            Category = API.Dtos.FacilityCategory.Restaurant
        };

        // Act & Assert
        Should.Throw<EntityValidationException>(() => controller.Create(duplicateEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // First create a facility to update
        var originalEntity = new FacilityDto
        {
            Name = "Original Facility " + Guid.NewGuid().ToString().Substring(0, 8),
            Latitude = 40.779437,
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.WC
        };
        var created = ((ObjectResult)controller.Create(originalEntity).Result)?.Value as FacilityDto;
        
        // Now update it
        var updatedEntity = new FacilityDto
        {
            Id = created.Id,
            Name = "Updated Facility " + Guid.NewGuid().ToString().Substring(0, 8),
            Latitude = 40.785091,
            Longitude = -73.968285,
            Category = API.Dtos.FacilityCategory.Restaurant
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as FacilityDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(updatedEntity.Name);
        result.UpdatedAt.ShouldNotBeNull();

        // Assert - Database
        var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Id == created.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedEntity.Name);
        storedEntity.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new FacilityDto
        {
            Id = -1000,
            Name = "Non-existent Facility",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.Other
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
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // First create a facility to delete
        var entityToDelete = new FacilityDto
        {
            Name = "Facility To Delete " + Guid.NewGuid().ToString().Substring(0, 8),
            Latitude = 40.779437,
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.Other
        };
        var created = ((ObjectResult)controller.Create(entityToDelete).Result)?.Value as FacilityDto;

        // Act
        var result = (OkResult)controller.Delete(created.Id);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Id == created.Id);
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
    
    private static FacilityController CreateController(IServiceScope scope)
    {
        return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
