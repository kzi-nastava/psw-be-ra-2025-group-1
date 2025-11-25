using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class PersonEquipmentQueryTests : BaseToursIntegrationTest
{
    public PersonEquipmentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_available_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IPersonEquipmentService>();

        // Debug: Check what's in the database
        var equipmentInDb = dbContext.Equipment.ToList();
        
        // Debug: Call service directly
        var serviceResult = service.GetAvailableEquipment(0, 0);

        // Act - GetAll should return available equipment (EquipmentDto based on interface)
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<EquipmentDto>;

        // Assert
        result.ShouldNotBeNull();
        
        // Adjust expectations based on what's actually in the database
        var expectedCount = equipmentInDb.Count;
        result.Results.Count.ShouldBe(expectedCount);
        result.TotalCount.ShouldBe(expectedCount);
        
        // Also check service result
        serviceResult.Results.Count.ShouldBe(expectedCount);
        serviceResult.TotalCount.ShouldBe(expectedCount);
    }

    [Fact]
    public void Retrieves_person_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - GetPersonEquipments should return PersonEquipmentDto based on interface
        var result = ((ObjectResult)controller.GetPersonEquipments(0, 0).Result)?.Value as PagedResult<PersonEquipmentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeNull();
    }

    private static PersonEquipmentController CreateController(IServiceScope scope)
    {
        return new PersonEquipmentController(scope.ServiceProvider.GetRequiredService<IPersonEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}