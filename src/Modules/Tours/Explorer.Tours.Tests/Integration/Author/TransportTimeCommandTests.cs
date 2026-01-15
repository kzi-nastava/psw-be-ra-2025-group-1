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

    private static TourController CreateController(IServiceScope scope, string userId = "-1")
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }

    private Tour EnsureDraftTourExists(ToursContext dbContext, long creatorId)
    {
        // Try to find existing tour with matching creator
        var tour = dbContext.Tour
            .Include(t => t.TransportTimes)
            .FirstOrDefault(t => t.Status == TourStatus.Draft && t.CreatorId == creatorId);
        
        if (tour == null)
        {
            // Create a new tour with matching creator ID
            tour = new Tour(creatorId, "Test Tour", "Test Description", 5, new[] { "test" }, TourStatus.Draft, 100);
            dbContext.Tour.Add(tour);
            dbContext.SaveChanges();
        }
        
        return tour;
    }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // Use creator ID that matches the controller context
        long creatorId = -1;
        var tour = EnsureDraftTourExists(dbContext, creatorId);
        var controller = CreateController(scope, creatorId.ToString());

        var newEntity = new TransportTimeDto
        {
            Type = TransportTypeDto.Car,
            Duration = 45,
        };

        // Act
        var response = controller.AddTransportTime(tour.Id, newEntity);
        var result = (response.Result as ObjectResult)?.Value as TransportTimeDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Type.ShouldBe(newEntity.Type);
        result.Duration.ShouldBe(newEntity.Duration);

        // Assert - Database
        var storedEntity = dbContext.TransportTime
            .AsNoTracking()
            .FirstOrDefault(i => i.Duration == newEntity.Duration && i.Type == TransportType.Car);
        storedEntity.ShouldNotBeNull();
        storedEntity.Duration.ShouldBe(result.Duration);
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // Use creator ID that matches the controller context
        long creatorId = -1;
        var tour = EnsureDraftTourExists(dbContext, creatorId);
        
        // Ensure tour has a transport time to update
        if (!tour.TransportTimes.Any())
        {
            var tt = tour.AddTransportTime(new TransportTime(TransportType.Foot, 10));
            dbContext.SaveChanges();
        }
        
        // Refresh tour to get the transport time with ID
        tour = dbContext.Tour
            .Include(t => t.TransportTimes)
            .First(t => t.Id == tour.Id);
        
        var existingTransport = tour.TransportTimes.First();
        var controller = CreateController(scope, creatorId.ToString());
        
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
        var storedEntity = dbContext.TransportTime
            .AsNoTracking()
            .FirstOrDefault(i => i.Id == existingTransport.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Duration.ShouldBe(updatedEntity.Duration);
        storedEntity.Type.ShouldBe(TransportType.Bike);
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Use creator ID that matches the controller context
        long creatorId = -1;
        var tour = EnsureDraftTourExists(dbContext, creatorId);

        // Ensure tour has a transport time to delete
        if (!tour.TransportTimes.Any())
        {
            var tt = tour.AddTransportTime(new TransportTime(TransportType.Foot, 10));
            dbContext.SaveChanges();
        }

        // Refresh tour to get the transport time with ID
        tour = dbContext.Tour
            .Include(t => t.TransportTimes)
            .First(t => t.Id == tour.Id);

        var existingTransport = tour.TransportTimes.First();
        var controller = CreateController(scope, creatorId.ToString());
        var transportTimeId = existingTransport.Id;

        // Act
        var result = controller.DeleteTransportTime(tour.Id, transportTimeId);

        // Assert - Response
        result.ShouldNotBeNull();
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.TransportTime
            .AsNoTracking()
            .FirstOrDefault(i => i.Id == transportTimeId);
        storedEntity.ShouldBeNull();
    }
}