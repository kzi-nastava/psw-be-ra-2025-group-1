using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class FacilityQueryTests : BaseToursIntegrationTest
{
    public FacilityQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<FacilityDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Retrieves_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        // First create a facility
        var newFacility = new FacilityDto
        {
            Name = "Test Facility " + Guid.NewGuid().ToString().Substring(0, 8),
            Latitude = 40.779437,
            Longitude = -73.963244,
            Category = API.Dtos.FacilityCategory.Store
        };
        var created = ((ObjectResult)controller.Create(newFacility).Result)?.Value as FacilityDto;

        // Act
        var result = ((ObjectResult)controller.GetById(created.Id).Result)?.Value as FacilityDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(newFacility.Name);
        result.Latitude.ShouldBe(newFacility.Latitude);
        result.Longitude.ShouldBe(newFacility.Longitude);
    }

    private static FacilityController CreateController(IServiceScope scope)
    {
        return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
