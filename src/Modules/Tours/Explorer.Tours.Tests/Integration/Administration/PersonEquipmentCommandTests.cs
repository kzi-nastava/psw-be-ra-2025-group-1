using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        
        var newEntity = new PersonEquipmentDto
        {
            PersonId = -1, // Controller will override this anyway
            EquipmentId = -1
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as PersonEquipmentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.PersonId.ShouldBe(-1);
        result.EquipmentId.ShouldBe(-1);
        result.Id.ShouldNotBe(0);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new PersonEquipmentDto
        {
            PersonId = -1, // This will be overridden by controller to -1
            EquipmentId = 9999 // Non-existent equipment ID
        };

        Should.Throw<NotFoundException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // First, create a person equipment to delete
        var testEntity = new PersonEquipmentDto
        {
            PersonId = -1,
            EquipmentId = -1
        };
        controller.Create(testEntity);

        // Act - Delete takes only equipmentId, personId comes from JWT token
        var result = (OkResult)controller.Delete(-1);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.PersonEquipment.FirstOrDefault(i => i.PersonId == -1 && i.EquipmentId == -1);
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