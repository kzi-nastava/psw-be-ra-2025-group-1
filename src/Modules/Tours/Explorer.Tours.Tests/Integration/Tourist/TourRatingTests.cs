using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
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
public class TourRatingTests : BaseToursIntegrationTest
{
    public TourRatingTests(ToursTestFactory factory) : base(factory)
    {
    }

    private static TourRatingController CreateController(IServiceScope scope, long touristId)
    {
        var controller = new TourRatingController(
            scope.ServiceProvider.GetRequiredService<ITourRatingService>(),
            scope.ServiceProvider.GetRequiredService<ITourRatingReactionService>());

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
    public void Creates_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 5,
            Comment = "Excellent tour, highly recommended!",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var actionResult = controller.Create(ratingDto);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as TourRatingDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.UserId.ShouldBe(-11);
        result.TourExecutionId.ShouldBe(-10811);
        result.Stars.ShouldBe(5);
        result.Comment.ShouldBe("Excellent tour, highly recommended!");

        // Database check
        dbContext.ChangeTracker.Clear();
        var storedRating = dbContext.TourRatings.FirstOrDefault(tr => tr.Id == result.Id);
        storedRating.ShouldNotBeNull();
        storedRating.UserId.ShouldBe(-11);
        storedRating.Stars.ShouldBe(5);
    }

    [Fact]
    public void Gets_all_ratings_paged()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        // Act
        var actionResult = controller.GetAll(0, 10);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as PagedResult<TourRatingDto>;

        result.ShouldNotBeNull();
        result.Results.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Gets_my_ratings()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        // Create a rating first
        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 4,
            Comment = "Good tour",
            CreatedAt = DateTime.UtcNow
        };
        var createResult = controller.Create(ratingDto);
        createResult.Result.ShouldBeOfType<OkObjectResult>();

        // Act
        var actionResult = controller.GetMyRatings(0, 10);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as PagedResult<TourRatingDto>;

        result.ShouldNotBeNull();
        result.Results.ShouldNotBeNull();
        result.Results.All(r => r.UserId == -11).ShouldBeTrue();
    }

    [Fact]
    public void Gets_ratings_by_tour_execution()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        // Act
        var actionResult = controller.GetByTourExecution(-10811, 0, 10);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as PagedResult<TourRatingDto>;

        result.ShouldNotBeNull();
        result.Results.ShouldNotBeNull();
    }

    [Fact]
    public void Updates_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Create a rating first
        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 3,
            Comment = "Average tour",
            CreatedAt = DateTime.UtcNow
        };
        var createResult = controller.Create(ratingDto);
        var created = ((OkObjectResult)createResult.Result)?.Value as TourRatingDto;

        // Update the rating
        var updatedDto = new TourRatingDto
        {
            Id = created.Id,
            TourExecutionId = -10811,
            Stars = 5,
            Comment = "Actually, it was amazing!",
            CreatedAt = created.CreatedAt
        };

        // Act
        var actionResult = controller.Update(created.Id, updatedDto);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as TourRatingDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Stars.ShouldBe(5);
        result.Comment.ShouldBe("Actually, it was amazing!");

        // Database check
        dbContext.ChangeTracker.Clear();
        var storedRating = dbContext.TourRatings.FirstOrDefault(tr => tr.Id == result.Id);
        storedRating.ShouldNotBeNull();
        storedRating.Stars.ShouldBe(5);
        storedRating.Comment.ShouldBe("Actually, it was amazing!");
    }

    [Fact]
    public void Cannot_update_other_users_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller1 = CreateController(scope, -11);
        var controller2 = CreateController(scope, -12); // Different user

        // Create a rating with user -11
        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 4,
            Comment = "Nice tour",
            CreatedAt = DateTime.UtcNow
        };
        var createResult = controller1.Create(ratingDto);
        var created = ((OkObjectResult)createResult.Result)?.Value as TourRatingDto;

        // Try to update with different user (-12)
        var updatedDto = new TourRatingDto
        {
            Id = created.Id,
            TourExecutionId = -10811,
            Stars = 1,
            Comment = "Trying to sabotage!",
            CreatedAt = created.CreatedAt
        };

        // Act
        var result = controller2.Update(created.Id, updatedDto);

        // Assert
        result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public void Deletes_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Create a rating first
        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 3,
            Comment = "Will delete this",
            CreatedAt = DateTime.UtcNow
        };
        var createResult = controller.Create(ratingDto);
        var created = ((OkObjectResult)createResult.Result)?.Value as TourRatingDto;

        // Act
        var deleteResult = controller.Delete(created.Id);

        // Assert
        deleteResult.ShouldBeOfType<OkResult>();

        // Database check
        dbContext.ChangeTracker.Clear();
        var storedRating = dbContext.TourRatings.FirstOrDefault(tr => tr.Id == created.Id);
        storedRating.ShouldBeNull();
    }

    [Fact]
    public void Cannot_delete_other_users_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller1 = CreateController(scope, -11);
        var controller2 = CreateController(scope, -12); // Different user

        // Create a rating with user -11
        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 4,
            Comment = "My rating",
            CreatedAt = DateTime.UtcNow
        };
        var createResult = controller1.Create(ratingDto);
        var created = ((OkObjectResult)createResult.Result)?.Value as TourRatingDto;

        // Try to delete with different user
        // Act
        var result = controller2.Delete(created.Id);

        // Assert
        result.ShouldBeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public void Adds_thumbs_up_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller1 = CreateController(scope, -11);
        var controller2 = CreateController(scope, -12); // Different user to react

        // Create a rating with user -11
        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 5,
            Comment = "Great tour!",
            CreatedAt = DateTime.UtcNow
        };
        var createResult = controller1.Create(ratingDto);
        var created = ((OkObjectResult)createResult.Result)?.Value as TourRatingDto;

        // Act - User -12 adds thumbs up
        var actionResult = controller2.ThumbsUp(created.Id);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as TourRatingDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
    }

    [Fact]
    public void Removes_thumbs_up_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller1 = CreateController(scope, -11);
        var controller2 = CreateController(scope, -12);

        // Create a rating
        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 5,
            Comment = "Excellent!",
            CreatedAt = DateTime.UtcNow
        };
        var createResult = controller1.Create(ratingDto);
        var created = ((OkObjectResult)createResult.Result)?.Value as TourRatingDto;

        // Add thumbs up
        controller2.ThumbsUp(created.Id);

        // Act - Remove thumbs up
        var actionResult = controller2.RemoveThumbsUp(created.Id);

        // Assert
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = ((OkObjectResult)actionResult.Result)?.Value as TourRatingDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
    }

    [Fact]
    public void Cannot_create_rating_with_invalid_rating_value()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        var ratingDto = new TourRatingDto
        {
            TourExecutionId = -10811,
            Stars = 6, // Invalid - ratings are 1-5
            Comment = "Too high rating",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = controller.Create(ratingDto);

        // Assert
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();

        var errorObj = badRequestResult.Value;
        errorObj.ShouldNotBeNull();
    }

    [Fact]
    public void Cannot_update_nonexistent_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        var ratingDto = new TourRatingDto
        {
            Id = 999999, // Non-existent ID
            TourExecutionId = -10811,
            Stars = 5,
            Comment = "This won't work",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = controller.Update(999999, ratingDto);

        // Assert
        result.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Cannot_delete_nonexistent_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        // Act
        var result = controller.Delete(999999);

        // Assert
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Cannot_add_thumbs_up_to_nonexistent_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        // Act
        var result = controller.ThumbsUp(999999);

        // Assert
        result.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Cannot_remove_thumbs_up_from_nonexistent_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        // Act
        var result = controller.RemoveThumbsUp(999999);

        // Assert
        result.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Cannot_thumbs_up_own_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11);

        // First create a rating as user -11
        var createResult = controller.Create(new TourRatingDto
        {
            UserId = -11,
            TourExecutionId = -10811,
            Stars = 5,
            Comment = "My own rating"
        });
        var createdRating = ((OkObjectResult)createResult.Result!).Value as TourRatingDto;

        // Act - Try to thumbs up own rating
        var result = controller.ThumbsUp(createdRating!.Id);

        // Assert
        result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
    }
}