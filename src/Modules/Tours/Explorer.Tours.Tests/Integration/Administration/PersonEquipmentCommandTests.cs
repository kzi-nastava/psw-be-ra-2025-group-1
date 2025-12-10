using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class PersonEquipmentCommandTests : BaseToursIntegrationTest
{
    public PersonEquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // Create equipment first to ensure it exists
        var equipmentService = scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Administration.IEquipmentService>();
        var equipment = equipmentService.Create(new EquipmentDto
        {
            Name = "Test Equipment " + Guid.NewGuid().ToString().Substring(0, 8),
            Description = "Equipment for testing"
        });
        
        var newEntity = new PersonEquipmentDto
        {
            PersonId = -1, // Controller will override this anyway
            EquipmentId = equipment.Id
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as PersonEquipmentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.PersonId.ShouldBe(-1);
        result.EquipmentId.ShouldBe(equipment.Id);
        result.Id.ShouldNotBe(0);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new PersonEquipmentDto
        {
            PersonId = 0, // This will be overridden by controller to -1
            EquipmentId = 9999 // Non-existent equipment ID
        };

        // Act & Assert - Should throw when trying to get non-existent equipment
        Should.Throw<NotFoundException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // First, create equipment
        var equipmentService = scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Administration.IEquipmentService>();
        var equipment = equipmentService.Create(new EquipmentDto
        {
            Name = "Equipment to Delete " + Guid.NewGuid().ToString().Substring(0, 8),
            Description = "This equipment will be deleted"
        });

        // Then create person equipment to delete
        var testEntity = new PersonEquipmentDto
        {
            PersonId = -1,
            EquipmentId = equipment.Id
        };
        controller.Create(testEntity);

        // Act - Delete takes only equipmentId, personId comes from JWT token
        var result = (OkResult)controller.Delete(equipment.Id);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.PersonEquipment.FirstOrDefault(i => i.PersonId == -1 && i.EquipmentId == equipment.Id);
        storedEntity.ShouldBeNull();
    }
    
    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert - Delete with non-existent equipment ID
        Should.Throw<NotFoundException>(() => controller.Delete(-9999));
    }
    
    private static PersonEquipmentController CreateController(IServiceScope scope)
    {
        return new PersonEquipmentController(scope.ServiceProvider.GetRequiredService<IPersonEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}