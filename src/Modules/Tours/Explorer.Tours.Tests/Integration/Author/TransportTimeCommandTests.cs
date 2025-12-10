using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TransportTimeCommandTests : BaseToursIntegrationTest
{
    public TransportTimeCommandTests(ToursTestFactory factory) : base(factory)
    {
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>(), scope.ServiceProvider.GetRequiredService<ITransportTimeService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new TransportTimeDto
        {
            Type = TransportTypeDto.Car,
            Duration = 45,
            TourId = -15
        };

        // Act
        var result = ((ObjectResult)controller.CreateTransportTime(newEntity).Result)?.Value as TransportTimeDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Type.ShouldBe(newEntity.Type);
        result.Duration.ShouldBe(newEntity.Duration);
        result.TourId.ShouldBe(newEntity.TourId);

        // Assert - Database
        var storedEntity = dbContext.TransportTime.FirstOrDefault(i => i.Duration == newEntity.Duration && i.TourId == newEntity.TourId);
        storedEntity.ShouldNotBeNull();
        storedEntity.Duration.ShouldBe(result.Duration);
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new TransportTimeDto
        {
            Id = -1,
            Type = TransportTypeDto.Bike,
            Duration = 20,
            TourId = -15
        };

        // Act
        var result = ((ObjectResult)controller.UpdateTransportTime(updatedEntity).Result)?.Value as TransportTimeDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Type.ShouldBe(updatedEntity.Type);
        result.Duration.ShouldBe(updatedEntity.Duration);
        result.TourId.ShouldBe(updatedEntity.TourId);

        // Assert - Database
        var storedEntity = dbContext.TransportTime.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Duration.ShouldBe(updatedEntity.Duration);
        storedEntity.Type.ShouldBe(Explorer.Tours.Core.Domain.TransportType.Bike);
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var transportTimeId = -2;

        // Act
        var result = (OkResult)controller.DeleteTransportTime(transportTimeId).Result;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.TransportTime.FirstOrDefault(i => i.Id == transportTimeId);
        storedEntity.ShouldBeNull();
    }
}
