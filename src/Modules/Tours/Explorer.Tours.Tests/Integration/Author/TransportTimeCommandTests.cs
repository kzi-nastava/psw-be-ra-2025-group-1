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
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = new Tour(-1, "Temp Tour", "Desc", 1, new[] { "tag" }, TourStatus.Draft, 100);
        dbContext.Tour.Add(tour);
        dbContext.SaveChanges();
        var tourId = tour.Id;

        var newEntity = new TransportTimeDto
        {
            Type = TransportTypeDto.Car,
            Duration = 45,
        };

        var response = controller.AddTransportTime(tourId, newEntity);
        var result = (response.Result as ObjectResult)?.Value as TransportTimeDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Type.ShouldBe(newEntity.Type);
        result.Duration.ShouldBe(newEntity.Duration);

        var storedEntity = dbContext.TransportTime.FirstOrDefault(i => i.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Duration.ShouldBe(result.Duration);
        storedEntity.Type.ShouldBe(TransportType.Car);

        var updatedTour = dbContext.Tour.Include(t => t.TransportTimes).FirstOrDefault(t => t.Id == tourId);
        updatedTour.ShouldNotBeNull();
        updatedTour.TransportTimes.ShouldContain(tt => tt.Id == result.Id);
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = new Tour(-1, "Temp Tour", "Desc", 1, new[] { "tag" }, TourStatus.Draft, 100);
        dbContext.Tour.Add(tour);
        var transportTime = tour.AddTransportTime(new TransportTime(TransportType.Foot, 10));
        dbContext.SaveChanges();

        var updatedEntity = new TransportTimeDto
        {
            Id = transportTime.Id,
            Type = TransportTypeDto.Bike,
            Duration = 20
        };

        var response = controller.UpdateTransportTime(tour.Id, transportTime.Id, updatedEntity);
        var result = (response.Result as ObjectResult)?.Value as TransportTimeDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(transportTime.Id);
        result.Type.ShouldBe(updatedEntity.Type);
        result.Duration.ShouldBe(updatedEntity.Duration);

        var storedEntity = dbContext.TransportTime.AsNoTracking().FirstOrDefault(i => i.Id == transportTime.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Duration.ShouldBe(updatedEntity.Duration);
        storedEntity.Type.ShouldBe(TransportType.Bike);
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = new Tour(-1, "Temp Tour", "Desc", 1, new[] { "tag" }, TourStatus.Draft, 100);
        dbContext.Tour.Add(tour);
        var transportTime = tour.AddTransportTime(new TransportTime(TransportType.Foot, 10));
        dbContext.SaveChanges();
        var transportTimeId = transportTime.Id;

        var result = controller.DeleteTransportTime(tour.Id, transportTimeId) as OkResult;
        dbContext.SaveChanges();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var storedEntity = dbContext.TransportTime.AsNoTracking().FirstOrDefault(i => i.Id == transportTimeId);
        storedEntity.ShouldBeNull();
    }
}