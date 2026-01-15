using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Dtos.Enums;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory)
    {
    }
    private static TourController CreateController(IServiceScope scope, string userId = "-1")
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new CreateTourDto { CreatorId = 1, Title = "New Tour", Description = "A new tour", Difficulty = 3, Tags = new[] { "tag1", "tag2" }, Status = TourStatusDto.Draft, Price = 99.99 };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.CreatorId.ShouldNotBe(0);
        result.Title.ShouldBe(newEntity.Title);
        result.Description.ShouldBe(newEntity.Description);
        result.Difficulty.ShouldBe(newEntity.Difficulty);
        result.Status.ShouldBe(newEntity.Status);
        result.Price.ShouldBe(newEntity.Price);

        // Assert - Database
        var storedEntity = dbContext.Tour.FirstOrDefault(i => i.Title == newEntity.Title);
        storedEntity.ShouldNotBeNull();
        storedEntity.Title.ShouldBe(result.Title);
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Added logic to check if the object exists
        // This is useful so that the test passes even when ran with other test or by it's own
        var stored = dbContext.Tour.FirstOrDefault(i => i.Title == "New Tour");
        if (stored == null)
        {
            var newEntity = new CreateTourDto { CreatorId = 1, Title = "To Update", Description = "Will be updated", Difficulty = 2, Tags = new[] { "t1" }, Status = TourStatusDto.Draft, Price = 50.0 };

            var created = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;
            created.ShouldNotBeNull();

            stored = dbContext.Tour.FirstOrDefault(i => i.Title == newEntity.Title);
        }
        var id = stored.Id;

        var updatedDto = new TourDto { CreatorId = stored.CreatorId, Title = "Updated Title", Description = "Updated description", Difficulty = 5, Tags = new[] { "updated" }, Status = TourStatusDto.Published, Price = 150.0 };

        // Logging in fake user
        var identity = new ClaimsIdentity(new[]
{
                new Claim("id", "1"),
                new Claim("personId", "1"),
                new Claim(ClaimTypes.Role, "author")
            }, "TestAuthentication");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // Act
        var updateResult = ((ObjectResult)controller.Update(id, updatedDto).Result)?.Value as TourDto;

        // Assert - Response
        updateResult.ShouldNotBeNull();
        updateResult.Title.ShouldBe("Updated Title");
        updateResult.Description.ShouldBe("Updated description");
        updateResult.Difficulty.ShouldBe(5);
        updateResult.Price.ShouldBe(150.0);

        // Assert - Database
        dbContext.Entry(stored).State = EntityState.Detached;
        var storedUpdated = dbContext.Tour.FirstOrDefault(i => i.Id == id);
        storedUpdated.ShouldNotBeNull();
        storedUpdated.Title.ShouldBe("Updated Title");
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new CreateTourDto { CreatorId = 1, Title = "To Delete", Description = "Will be deleted", Difficulty = 1, Tags = new[] { "del" }, Status = TourStatusDto.Draft, Price = 10.0 };

        var created = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        var stored = dbContext.Tour.FirstOrDefault(i => i.Title == newEntity.Title);
        stored.ShouldNotBeNull();
        var id = stored.Id;

        // Logging in fake user
        var identity = new ClaimsIdentity(new[]
{
                new Claim("id", "1"),
                new Claim("personId", "1"),
                new Claim(ClaimTypes.Role, "author")
            }, "TestAuthentication");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // Act
        var deleteAction = controller.Delete(id);

        // Assert - Database
        var afterDelete = dbContext.Tour.FirstOrDefault(i => i.Id == id);
        afterDelete.ShouldBeNull();
    }

    [Theory]
    [InlineData("-1", -15, "Title", 200)] // Valid
    [InlineData("-1", -15, "", 400)] // Invalid title - Service throws ArgumentException, caught as BadRequest
    [InlineData("-1", -19, "Title", 401)] // Unauthorized - Service throws InvalidOperationException, caught as Unauthorized
    [InlineData("-1", -55, "Title", 404)] // Tour not found - Service throws KeyNotFoundException, caught as NotFound
    public void Adds_keypoint(string authorId, long tourId, string keypointTitle, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newKeypoint = new KeypointDto
        {
            Title = keypointTitle,
            Description = "Description",
            Secret = "Hidden",
            Latitude = 44.8,
            Longitude = 20.4
        };

        // Act
        var result = controller.AddKeypoint(tourId, newKeypoint);

        // Assert
        if (expectedStatus == 200)
        {
            result.Result.ShouldBeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result.Result;
            okResult.StatusCode.ShouldBe(200);

            var storedTour = dbContext.Tour.First(t => t.Id == tourId);
            storedTour.Keypoints.ShouldContain(kp => kp.Title == newKeypoint.Title);
        }
        else if (expectedStatus == 400)
        {
            // Service throws ArgumentException for validation errors
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }
        else if (expectedStatus == 401)
        {
            // Service throws InvalidOperationException for authorization failures
            result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
        }
        else if (expectedStatus == 404)
        {
            // Service throws KeyNotFoundException
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
        }
    }


    [Theory]
    [InlineData("-1", -15, -2, "Updated OK", 200)] // Valid
    [InlineData("-1", -15, -2, "", 400)] // Invalid title - Service throws ArgumentException, caught as BadRequest
    [InlineData("-1", -19, -2, "Title", 401)] // Unauthorized - Service throws InvalidOperationException, caught as Unauthorized
    [InlineData("-1", -15, 77, "Title", 404)] // Keypoint not found - Service throws KeyNotFoundException, caught as NotFound
    [InlineData("-1", -77, -2, "Title", 404)] // Tour not found - Service throws KeyNotFoundException, caught as NotFound
    public void Updates_keypoint(
        string authorId,
        long tourId,
        long keypointId,
        string newTitle,
        int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updated = new KeypointDto
        {
            Id = keypointId,
            Title = newTitle,
            Description = "Updated Desc",
            Secret = "Updated Secret",
            Latitude = 44.1,
            Longitude = 21.0
        };

        // Act
        var result = controller.UpdateKeypoint(tourId, keypointId, updated);

        // Assert
        if (expectedStatus == 200)
        {
            result.Result.ShouldBeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result.Result;
            okResult.StatusCode.ShouldBe(200);

            var storedTour = dbContext.Tour.First(t => t.Id == tourId);
            var kp = storedTour.Keypoints.FirstOrDefault(k => k.Id == keypointId);
            kp.ShouldNotBeNull();
            kp.Title.ShouldBe(newTitle);
        }
        else if (expectedStatus == 400)
        {
            // Service throws ArgumentException for validation errors
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }
        else if (expectedStatus == 401)
        {
            // Service throws InvalidOperationException for authorization failures
            result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
        }
        else if (expectedStatus == 404)
        {
            // Service throws KeyNotFoundException
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
        }
    }

    [Theory]
    [InlineData("-1", -15, -2, 200)] // Valid
    [InlineData("-2", -15, -2, 401)] // Unauthorized - tuđi korisnik pokušava da obriše
    [InlineData("-1", -15, 77, 404)] // Keypoint not found - Service throws KeyNotFoundException, caught as NotFound
    [InlineData("-1", -77, -2, 404)] // Tour not found - Service throws KeyNotFoundException, caught as NotFound
    public void Deletes_keypoint(string authorId, long tourId, long keypointId, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = controller.DeleteKeypoint(tourId, keypointId);

        // Assert
        if (expectedStatus == 200)
        {
            result.ShouldBeOfType<OkObjectResult>();

            // Assert - database
            dbContext.ChangeTracker.Clear();
            var storedTour = dbContext.Tour
                .Include(t => t.Keypoints)
                .FirstOrDefault(t => t.Id == tourId);
            
            if (storedTour != null)
            {
                var kp = storedTour.Keypoints.FirstOrDefault(k => k.Id == keypointId);
                kp.ShouldBeNull();
            }
        }
        else if (expectedStatus == 401)
        {
            // Service throws InvalidOperationException for authorization failures
            result.ShouldBeOfType<UnauthorizedObjectResult>();
        }
        else if (expectedStatus == 404)
        {
            // Service throws KeyNotFoundException
            result.ShouldBeOfType<NotFoundObjectResult>();
        }
    }

    [Theory]
    [InlineData("-1", -5, -2, 200)] // Valid
    [InlineData("-2", -5, -3, 401)] // Unauthorized - tuđi korisnik pokušava da doda opremu
    [InlineData("-1", -5, -4, 400)] // Already exists - Service throws ArgumentException/InvalidOperationException, caught as BadRequest
    [InlineData("-1", -5, -9999999, 404)] // Equipment not found - Service throws KeyNotFoundException, caught as NotFound
    [InlineData("-1", -9999999, -1, 404)] // Tour not found - Service throws KeyNotFoundException, caught as NotFound
    public void Adds_equipment_to_tour(string authorId, long tourId, long equipmentId, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var response = controller.AddEquipment(tourId, equipmentId);

        // Assert
        if (expectedStatus == 200)
        {
            response.Result.ShouldBeOfType<OkObjectResult>();

            // Assert – Database
            var stored = dbContext.Tour
                .Include(t => t.Equipment)
                .First(t => t.Id == tourId);
            stored.Equipment.ShouldContain(e => e.Id == equipmentId);
        }
        else if (expectedStatus == 400)
        {
            // Service throws ArgumentException for validation/business rule violations
            response.Result.ShouldBeOfType<BadRequestObjectResult>();
        }
        else if (expectedStatus == 401)
        {
            // Service throws InvalidOperationException for authorization failures
            response.Result.ShouldBeOfType<UnauthorizedObjectResult>();
        }
        else if (expectedStatus == 404)
        {
            // Service throws KeyNotFoundException
            response.Result.ShouldBeOfType<NotFoundObjectResult>();
        }
    }

    [Theory]
    [InlineData("-1", -5, -1, 200, true)]   // Valid - equipment exists on tour
    [InlineData("-1", -5, -3, 400, false)]  // Equipment not on tour - Service throws ArgumentException/InvalidOperationException, caught as BadRequest
    [InlineData("-1", -5, -9999999, 404, false)]  // Equipment doesn't exist - Service throws KeyNotFoundException, caught as NotFound
    [InlineData("-1", -9999999, -1, 404, false)]  // Tour doesn't exist - Service throws KeyNotFoundException, caught as NotFound
    public void Removes_equipment_from_tour(string authorId, long tourId, long equipmentId, int expectedStatus, bool shouldHaveEquipment)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        if (shouldHaveEquipment && expectedStatus == 200)
        {
            // Ensure the tour actually has the equipment before removing it
            var tour = dbContext.Tour
                .Include(t => t.Equipment)
                .First(t => t.Id == tourId);

            if (!tour.Equipment.Any(e => e.Id == equipmentId))
            {
                var equipment = dbContext.Equipment.First(e => e.Id == equipmentId);
                tour.Equipment.Add(equipment);
                dbContext.SaveChanges();
            }
        }

        // Act
        var response = controller.RemoveEquipment(tourId, equipmentId);

        // Assert
        if (expectedStatus == 200)
        {
            response.Result.ShouldBeOfType<OkObjectResult>();

            // Assert – Database
            var stored = dbContext.Tour
                .Include(t => t.Equipment)
                .First(t => t.Id == tourId);

            stored.Equipment.ShouldNotContain(e => e.Id == equipmentId);
        }
        else if (expectedStatus == 400)
        {
            // Service throws ArgumentException or InvalidOperationException for business rule violations
            response.Result.ShouldBeOfType<BadRequestObjectResult>();
        }
        else if (expectedStatus == 404)
        {
            // Service throws KeyNotFoundException
            response.Result.ShouldBeOfType<NotFoundObjectResult>();
        }
    }

}