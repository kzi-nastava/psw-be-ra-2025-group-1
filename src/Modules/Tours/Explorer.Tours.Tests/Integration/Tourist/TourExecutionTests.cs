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
        var controller = CreateController(scope, -21);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var startTourDto = new StartTourDto
        {
            TourId = -1,
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };

        // Act
        var actionResult = controller.StartTour(startTourDto);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TouristId.ShouldBe(-21);
        result.TourId.ShouldBe(-1);
        result.Status.ShouldBe(TourExecutionStatusDto.InProgress);
        result.PercentageCompleted.ShouldBe(0);

        // Database check
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
        var controller = CreateController(scope, -22);

        // Start first tour
        var firstTour = new StartTourDto
        {
            TourId = -1,
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };
        var firstResult = controller.StartTour(firstTour);
        firstResult.Result.ShouldBeOfType<OkObjectResult>();

        // Try to start the same tour again (or any tour while one is active)
        var secondTour = new StartTourDto
        {
            TourId = -1,
            InitialLatitude = 45.2500,
            InitialLongitude = 19.8300
        };
        var result = controller.StartTour(secondTour);

        // Assert
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();

        var errorObj = badRequestResult.Value;
        errorObj.ShouldNotBeNull();
        var errorProp = errorObj.GetType().GetProperty("error");
        var errorMessage = errorProp?.GetValue(errorObj)?.ToString();
        errorMessage.ShouldContain("already has an active tour");
    }

    [Fact]
    public void Gets_active_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -23);

        // Start a tour
        var startTourDto = new StartTourDto
        {
            TourId = -1,
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };
        var startedResult = controller.StartTour(startTourDto);
        startedResult.Result.ShouldBeOfType<OkObjectResult>();
        var started = ((OkObjectResult)startedResult.Result)?.Value as TourExecutionDto;

        // Act
        var getActiveResult = controller.GetActiveTour();

        // Assert
        getActiveResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)getActiveResult.Result)?.Value as TourExecutionDto;

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

        // Clean up any existing active tour for this tourist
        try
        {
            var activeResult = controller.GetActiveTour();
            if (activeResult.Result is OkObjectResult okResult && okResult.Value is TourExecutionDto activeDto)
            {
                controller.CompleteTour(activeDto.Id);
            }
        }
        catch { /* No active tour */ }

        // Start a new tour
        var startTourDto = new StartTourDto
        {
            TourId = -1,
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };
        var startedResult = controller.StartTour(startTourDto);
        startedResult.Result.ShouldBeOfType<OkObjectResult>();
        var started = ((OkObjectResult)startedResult.Result)?.Value as TourExecutionDto;

        // Act
        var completeResult = controller.CompleteTour(started.Id);

        // Assert
        completeResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)completeResult.Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe(TourExecutionStatusDto.Completed);
        result.PercentageCompleted.ShouldBe(100);
        result.EndTime.ShouldNotBeNull();

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
        var controller = CreateController(scope, -22);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Clean up any existing active tour
        try
        {
            var activeResult = controller.GetActiveTour();
            if (activeResult.Result is OkObjectResult okResult && okResult.Value is TourExecutionDto activeDto)
            {
                controller.AbandonTour(activeDto.Id);
            }
        }
        catch { /* No active tour */ }

        // Start a new tour
        var startTourDto = new StartTourDto
        {
            TourId = -1,
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };
        var startedResult = controller.StartTour(startTourDto);
        startedResult.Result.ShouldBeOfType<OkObjectResult>();
        var started = ((OkObjectResult)startedResult.Result)?.Value as TourExecutionDto;

        // Act
        var abandonResult = controller.AbandonTour(started.Id);

        // Assert
        abandonResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)abandonResult.Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe(TourExecutionStatusDto.Abandoned);
        result.EndTime.ShouldNotBeNull();

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
        var controller = CreateController(scope, -23);

        // Clean up and start fresh
        try
        {
            var activeResult = controller.GetActiveTour();
            if (activeResult.Result is OkObjectResult okResult && okResult.Value is TourExecutionDto activeDto)
            {
                controller.CompleteTour(activeDto.Id);
            }
        }
        catch { /* No active tour */ }

        // Start and complete a tour
        var startTourDto = new StartTourDto
        {
            TourId = -1,
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };
        var startedResult = controller.StartTour(startTourDto);
        startedResult.Result.ShouldBeOfType<OkObjectResult>();
        var started = ((OkObjectResult)startedResult.Result)?.Value as TourExecutionDto;

        controller.CompleteTour(started.Id);

        // Act
        var historyResult = controller.GetHistory();

        // Assert
        historyResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)historyResult.Result)?.Value as List<TourExecutionDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.Any(te => te.Id == started.Id).ShouldBeTrue();
    }

    [Fact]
    public void Cannot_start_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

        // Use tour -15 which is Draft (Status = 0) from e-tour.sql
        // The status check happens BEFORE purchase check, so we'll get the status error
        var startTourDto = new StartTourDto
        {
            TourId = -15,
            InitialLatitude = 45.2396,
            InitialLongitude = 19.8227
        };

        // Act
        var result = controller.StartTour(startTourDto);

        // Assert
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();

        var errorObj = badRequestResult.Value;
        errorObj.ShouldNotBeNull();
        var errorProp = errorObj.GetType().GetProperty("error");
        var errorMessage = errorProp?.GetValue(errorObj)?.ToString();
        errorMessage.ShouldContain("published or archived");
    }
}