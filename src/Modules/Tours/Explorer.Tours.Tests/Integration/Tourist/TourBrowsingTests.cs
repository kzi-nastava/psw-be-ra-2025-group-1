using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourBrowsingTests : BaseToursIntegrationTest
{
    public TourBrowsingTests(ToursTestFactory factory) : base(factory)
    {
    }

    private static TouristTourController CreateController(IServiceScope scope)
    {
        return new TouristTourController(
            scope.ServiceProvider.GetRequiredService<ITourBrowsingService>());
    }

    [Fact]
    public void Search_tours_by_title()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(0, 10, "Mountain", null);

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        tours.Results.Count.ShouldBeGreaterThan(0);
        tours.Results.ShouldAllBe(t => t.Title.Contains("Mountain", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Search_tours_by_description()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(0, 10, "beautiful", null);

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        tours.Results.Count.ShouldBeGreaterThan(0);
        tours.Results.ShouldAllBe(t => 
            t.Title.Contains("beautiful", StringComparison.OrdinalIgnoreCase) ||
            t.Description.Contains("beautiful", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Get_all_tours_without_search()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(0, 10, null, null);

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        tours.Results.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Search_tours_case_insensitive()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - lowercase search
        var result1 = controller.GetPublished(0, 10, "mountain", null);
        // Act - uppercase search
        var result2 = controller.GetPublished(0, 10, "MOUNTAIN", null);

        // Assert
        result1.Result.ShouldBeOfType<OkObjectResult>();
        result2.Result.ShouldBeOfType<OkObjectResult>();
        
        var tours1 = ((PagedResult<TourDto>)((OkObjectResult)result1.Result).Value!).Results;
        var tours2 = ((PagedResult<TourDto>)((OkObjectResult)result2.Result).Value!).Results;

        tours1.Count.ShouldBe(tours2.Count);
    }

    [Fact]
    public void Get_tours_sorted_by_tags()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(0, 10, null, "tags");

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        tours.Results.Count.ShouldBeGreaterThan(0);

        // Verify tours are sorted by tags alphabetically
        for (int i = 0; i < tours.Results.Count - 1; i++)
        {
            var tags1 = tours.Results[i].Tags.Length > 0 
                ? string.Join(",", tours.Results[i].Tags.OrderBy(t => t)) 
                : string.Empty;
            var tags2 = tours.Results[i + 1].Tags.Length > 0 
                ? string.Join(",", tours.Results[i + 1].Tags.OrderBy(t => t)) 
                : string.Empty;

            string.Compare(tags1, tags2, StringComparison.OrdinalIgnoreCase).ShouldBeLessThanOrEqualTo(0);
        }
    }

    [Fact]
    public void Get_tours_sorted_by_difficulty_ascending()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(0, 10, null, "difficulty-asc");

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        tours.Results.Count.ShouldBeGreaterThan(0);

        // Verify tours are sorted by difficulty in ascending order
        for (int i = 0; i < tours.Results.Count - 1; i++)
        {
            tours.Results[i].Difficulty.ShouldBeLessThanOrEqualTo(tours.Results[i + 1].Difficulty);
        }
    }

    [Fact]
    public void Get_tours_sorted_by_difficulty_descending()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(0, 10, null, "difficulty-desc");

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        tours.Results.Count.ShouldBeGreaterThan(0);

        // Verify tours are sorted by difficulty in descending order
        for (int i = 0; i < tours.Results.Count - 1; i++)
        {
            tours.Results[i].Difficulty.ShouldBeGreaterThanOrEqualTo(tours.Results[i + 1].Difficulty);
        }
    }

    [Fact]
    public void Search_and_sort_combined()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Search for "tour" and sort by difficulty
        var result = controller.GetPublished(0, 10, "tour", "difficulty-asc");

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        
        // Verify search worked
        tours.Results.ShouldAllBe(t => 
            t.Title.Contains("tour", StringComparison.OrdinalIgnoreCase) ||
            t.Description.Contains("tour", StringComparison.OrdinalIgnoreCase));

        // Verify sorting worked
        if (tours.Results.Count > 1)
        {
            for (int i = 0; i < tours.Results.Count - 1; i++)
            {
                tours.Results[i].Difficulty.ShouldBeLessThanOrEqualTo(tours.Results[i + 1].Difficulty);
            }
        }
    }

    [Fact]
    public void Search_with_pagination()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(1, 2, "tour", null);

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tours = (PagedResult<TourDto>)okResult.Value!;

        tours.ShouldNotBeNull();
        tours.Results.Count.ShouldBeLessThanOrEqualTo(2);
    }
}

