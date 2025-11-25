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
        
        // Check what PersonEquipment records exist
        var allPersonEquipment = dbContext.PersonEquipment.ToList();
        var targetPersonEquipment = dbContext.PersonEquipment.FirstOrDefault(pe => pe.PersonId == -1 && pe.EquipmentId == -1);
        
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
        
        if (targetPersonEquipment != null)
        {
            // If test data exists, should return existing record
            result.Id.ShouldBe(targetPersonEquipment.Id);
        }
        else
        {
            // If no test data, should create new record
            result.Id.ShouldNotBe(0);
            
            // Verify it was created in database
            var storedEntity = dbContext.PersonEquipment.FirstOrDefault(i => i.PersonId == -1 && i.EquipmentId == -1 && i.Id == result.Id);
            storedEntity.ShouldNotBeNull();
        }
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
            EquipmentId = 0 // Invalid equipment ID - should trigger validation
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-1, -1);

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

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000, -1000));
    }
    
    private static PersonEquipmentController CreateController(IServiceScope scope)
    {
        return new PersonEquipmentController(scope.ServiceProvider.GetRequiredService<IPersonEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}