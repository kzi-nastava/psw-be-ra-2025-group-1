using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
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
        
        var tour = dbContext.Tour.FirstOrDefault(t => t.Status == TourStatus.Draft);
        if (tour == null)
        {
            // Fallback: create a tour if none exists
             tour = new Tour(1, "Temp Tour", "Desc", 1, new[] { "tag" }, TourStatus.Draft, 100);
             dbContext.Tour.Add(tour);
             dbContext.SaveChanges();
        }
        var tourId = tour.Id;

        var newEntity = new TransportTimeDto
        {
            Type = TransportTypeDto.Car,
            Duration = 45,
        };

        // Act
        var response = controller.AddTransportTime(tourId, newEntity);
        var result = (response.Result as ObjectResult)?.Value as TransportTimeDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Type.ShouldBe(newEntity.Type);
        result.Duration.ShouldBe(newEntity.Duration);

        // Assert - Database
        var storedEntity = dbContext.TransportTime.FirstOrDefault(i => i.Duration == newEntity.Duration && i.Type == TransportType.Car);
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
        
        var existingTransport = dbContext.TransportTime.FirstOrDefault();
        if (existingTransport == null)
        {
             var wrapperTour = new Tour(1, "Temp Tour", "Desc", 1, new[] { "tag" }, TourStatus.Draft, 100);
             dbContext.Tour.Add(wrapperTour);
             var tt = wrapperTour.AddTransportTime(new TransportTime(TransportType.Foot, 10));
             dbContext.SaveChanges();
             existingTransport = tt;
        }

        var tour = dbContext.Tour.Include(t => t.TransportTimes).First(t => t.TransportTimes.Any(tt => tt.Id == existingTransport.Id));
        
        var updatedEntity = new TransportTimeDto
        {
            Id = existingTransport.Id,
            Type = TransportTypeDto.Bike,
            Duration = 20
        };

        // Act
        var response = controller.UpdateTransportTime(tour.Id, existingTransport.Id, updatedEntity);
        var result = (response.Result as ObjectResult)?.Value as TransportTimeDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(existingTransport.Id);
        result.Type.ShouldBe(updatedEntity.Type);
        result.Duration.ShouldBe(updatedEntity.Duration);

        // Assert - Database
        var storedEntity = dbContext.TransportTime.AsNoTracking().FirstOrDefault(i => i.Id == existingTransport.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Duration.ShouldBe(updatedEntity.Duration);
        storedEntity.Type.ShouldBe(TransportType.Bike);
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Find a transport time to delete
        var existingTransport = dbContext.TransportTime.FirstOrDefault();
        if (existingTransport == null)
        {
             var wrapperTour = new Tour(1, "Temp Tour", "Desc", 1, new[] { "tag" }, TourStatus.Draft, 100);
             dbContext.Tour.Add(wrapperTour);
             var tt = wrapperTour.AddTransportTime(new TransportTime(TransportType.Foot, 10));
             dbContext.SaveChanges();
             existingTransport = tt;
        }
        
        var tour = dbContext.Tour.Include(t => t.TransportTimes).First(t => t.TransportTimes.Any(tt => tt.Id == existingTransport.Id));
        var transportTimeId = existingTransport.Id;

        // Act
        var result = controller.DeleteTransportTime(tour.Id, transportTimeId) as OkResult;
        dbContext.SaveChanges();

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.TransportTime.AsNoTracking().FirstOrDefault(i => i.Id == transportTimeId);
        storedEntity.ShouldBeNull();
    }
}
