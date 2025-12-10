using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourExecutionTests : BaseToursIntegrationTest
{
    public TourExecutionTests(ToursTestFactory factory) : base(factory)
    {
    }

    private static TourExecutionController CreateController(IServiceScope scope, long touristId)
    {
        var controller = new TourExecutionController(
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", touristId.ToString()),
                    new Claim("id", touristId.ToString()),
                    new Claim(ClaimTypes.Role, "tourist")
                }, "test"))
            }
        };

        return controller;
    }

    [Fact]
    public void Starts_tour_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21); // Tourist ID from test data
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var startTourDto = new StartTourDto
        {
            TourId = -1, // Tour from e-tour.sql (must be Published/Archived)
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };

        // Act
        var result = ((ObjectResult)controller.StartTour(startTourDto).Result)?.Value as TourExecutionDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TouristId.ShouldBe(-21);
        result.TourId.ShouldBe(-1);
        result.Status.ShouldBe(TourExecutionStatusDto.InProgress);
        result.PercentageCompleted.ShouldBe(0);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == result.Id);
        storedExecution.ShouldNotBeNull();
        storedExecution.TouristId.ShouldBe(-21);
        storedExecution.TourId.ShouldBe(-1);
    }

    [Fact]
    public void Cannot_start_tour_when_another_is_active()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -22); // Use different tourist

        // Start first tour
        var firstTour = new StartTourDto { TourId = -1, InitialLatitude = 45.2396, InitialLongitude = 19.8227 };
        var firstResult = controller.StartTour(firstTour);
        firstResult.Result.ShouldBeOfType<OkObjectResult>(); // Verify first tour started successfully

        // Try to start second tour
        var secondTour = new StartTourDto { TourId = -2, InitialLatitude = 45.2396, InitialLongitude = 19.8227 };

        // Act
        var result = controller.StartTour(secondTour);

        // Assert
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        var errorValue = badRequestResult?.Value;
        errorValue.ShouldNotBeNull();
        errorValue.ToString().ShouldContain("already has an active tour");
    }

    [Fact]
    public void Gets_active_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -23); // Use different tourist

        // Start a tour
        var startTourDto = new StartTourDto { TourId = -1, InitialLatitude = 45.2396, InitialLongitude = 19.8227 };
        var started = ((ObjectResult)controller.StartTour(startTourDto).Result)?.Value as TourExecutionDto;

        // Act
        var result = ((ObjectResult)controller.GetActiveTour().Result)?.Value as TourExecutionDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(started.Id);
        result.Status.ShouldBe(TourExecutionStatusDto.InProgress);
    }

    [Fact]
    public void Completes_tour_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // First complete any existing active tour for this tourist
        try
        {
            var activeTour = controller.GetActiveTour();
            if (activeTour.Result is ObjectResult activeResult && activeResult.Value is TourExecutionDto activeDto)
            {
                controller.CompleteTour(activeDto.Id);
            }
        }
        catch { /* No active tour, that's fine */ }

        // Start a tour
        var startTourDto = new StartTourDto { TourId = -1, InitialLatitude = 45.2396, InitialLongitude = 19.8227 };
        var started = ((ObjectResult)controller.StartTour(startTourDto).Result)?.Value as TourExecutionDto;

        // Act
        var result = ((ObjectResult)controller.CompleteTour(started.Id).Result)?.Value as TourExecutionDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(TourExecutionStatusDto.Completed);
        result.PercentageCompleted.ShouldBe(100);
        result.EndTime.ShouldNotBeNull();

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == result.Id);
        storedExecution.ShouldNotBeNull();
        storedExecution.Status.ShouldBe(Core.Domain.TourExecutionStatus.Completed);
    }

    [Fact]
    public void Abandons_tour_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // First complete any existing active tour for this tourist
        try
        {
            var activeTour = controller.GetActiveTour();
            if (activeTour.Result is ObjectResult activeResult && activeResult.Value is TourExecutionDto activeDto)
            {
                controller.AbandonTour(activeDto.Id);
            }
        }
        catch { /* No active tour, that's fine */ }

        // Start a tour
        var startTourDto = new StartTourDto { TourId = -1, InitialLatitude = 45.2396, InitialLongitude = 19.8227 };
        var started = ((ObjectResult)controller.StartTour(startTourDto).Result)?.Value as TourExecutionDto;

        // Act
        var result = ((ObjectResult)controller.AbandonTour(started.Id).Result)?.Value as TourExecutionDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(TourExecutionStatusDto.Abandoned);
        result.EndTime.ShouldNotBeNull();

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == result.Id);
        storedExecution.ShouldNotBeNull();
        storedExecution.Status.ShouldBe(Core.Domain.TourExecutionStatus.Abandoned);
    }

    [Fact]
    public void Gets_tourist_execution_history()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

        // First complete any existing active tour for this tourist
        try
        {
            var activeTour = controller.GetActiveTour();
            if (activeTour.Result is ObjectResult activeResult && activeResult.Value is TourExecutionDto activeDto)
            {
                controller.CompleteTour(activeDto.Id);
            }
        }
        catch { /* No active tour, that's fine */ }

        // Start and complete a tour
        var startTourDto = new StartTourDto { TourId = -1, InitialLatitude = 45.2396, InitialLongitude = 19.8227 };
        var started = ((ObjectResult)controller.StartTour(startTourDto).Result)?.Value as TourExecutionDto;
        controller.CompleteTour(started.Id);

        // Act
        var result = ((ObjectResult)controller.GetHistory().Result)?.Value as List<TourExecutionDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.Any(te => te.Id == started.Id).ShouldBeTrue();
    }

    [Fact]
    public void Cannot_start_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -23); // Use different tourist to avoid conflicts

        // Tour -3 is Draft status (from test data)
        var startTourDto = new StartTourDto { TourId = -3, InitialLatitude = 45.2396, InitialLongitude = 19.8227 };

        // Act
        var result = controller.StartTour(startTourDto);

        // Assert
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        var errorValue = badRequestResult?.Value;
        errorValue.ShouldNotBeNull();
        errorValue.ToString().ShouldContain("published or archived");
    }
}

// Helper class for error responses
public class ErrorResponse
{
    public string Error { get; set; }
}
