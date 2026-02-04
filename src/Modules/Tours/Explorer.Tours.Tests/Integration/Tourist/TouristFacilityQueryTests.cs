using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristFacilityQueryTests : BaseToursIntegrationTest
{
    public TouristFacilityQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Tourist_retrieves_all_facilities()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<FacilityDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.All(f => !f.IsDeleted).ShouldBeTrue();
    }

    [Fact]
    public void Tourist_retrieves_facility_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        // Get all facilities first
        var allFacilities = ((ObjectResult)controller.GetAll().Result)?.Value as List<FacilityDto>;
        allFacilities.ShouldNotBeEmpty();
        
        var facilityId = allFacilities.First().Id;

        // Act
        var result = ((ObjectResult)controller.GetById(facilityId).Result)?.Value as FacilityDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(facilityId);
        result.Latitude.ShouldBeInRange(-90, 90);
        result.Longitude.ShouldBeInRange(-180, 180);
    }

    [Fact]
    public void Tourist_retrieves_facilities_by_category()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var adminFacilityService = scope.ServiceProvider.GetRequiredService<IFacilityService>();
        var category = FacilityCategory.Store;

        // Ensure at least one Store facility exists
        var existingStores = adminFacilityService.GetByCategory(category);
        if (!existingStores.Any())
        {
            // Create a test facility
            adminFacilityService.Create(new FacilityDto
            {
                Name = "Test Store " + Guid.NewGuid().ToString().Substring(0, 8),
                Latitude = 45.2551,
                Longitude = 19.8451,
                Category = category,
                CreatorId = -1,
                IsLocalPlace = false
            });
        }

        // Act
        var result = ((ObjectResult)controller.GetByCategory(category).Result)?.Value as List<FacilityDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty(); // Now we ensure there's at least one
        result.All(f => f.Category == category).ShouldBeTrue();
        result.All(f => !f.IsDeleted).ShouldBeTrue();
    }

    private static TouristFacilityController CreateController(IServiceScope scope)
    {
        return new TouristFacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
