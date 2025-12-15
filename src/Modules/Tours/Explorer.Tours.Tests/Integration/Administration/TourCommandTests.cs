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
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>(), scope.ServiceProvider.GetRequiredService<ITransportTimeService>())
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
        // THis is usefull so that the test passes even when ran with other test or by it's own
        var stored = dbContext.Tour.FirstOrDefault(i => i.Title == "New Tour");
        if(stored == null)
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
    [InlineData("-1", -15, "", 400)] // Invalid, keypoint title not added
    [InlineData("-1", -19, "Title", 403)] // Invalid, unauthorized
    [InlineData("-1", -55, "Title", 404)] // Invalid, tour doesn't exist   
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

        // Act & Assert
        if (expectedStatus == 200)
        {
            var result = (ObjectResult)controller.AddKeypoint(tourId, newKeypoint).Result;
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var storedTour = dbContext.Tour.First(t => t.Id == tourId);
            storedTour.Keypoints.ShouldContain(kp => kp.Title == newKeypoint.Title);
        }
        else
        {
            var ex = Should.Throw<Exception>(() => controller.AddKeypoint(tourId, newKeypoint));

            if (expectedStatus == 400)
                ex.ShouldBeOfType<ArgumentException>();   // invalid title / invalid lat/lon
            else if (expectedStatus == 403)
                ex.ShouldBeOfType<InvalidOperationException>(); // unauthorized
            else if (expectedStatus == 404)
                ex.ShouldBeOfType<NotFoundException>();   // tour not found
        }
    }
    

    [Theory] 
    [InlineData("-1", -5,-1, "Updated OK", 200)] // Valid
    [InlineData("-1", -5, -1, "", 400)] // Invalid, empty title
    [InlineData("-1", -19, 1, "Title", 403)] // Invalid, unauthorized
    [InlineData("-1", -5, 77, "Title", 404)] // Invalid, keypoint not found
    [InlineData("-1", -77, -1, "Title", 404)] // Invalid, tour not found
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

        if (expectedStatus == 200)
        {
            // Act
            var result = (ObjectResult)controller.UpdateKeypoint(tourId, keypointId, updated).Result;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(expectedStatus);

            var storedTour = dbContext.Tour.First(t => t.Id == tourId);
            var kp = storedTour.Keypoints.FirstOrDefault(k => k.Id == keypointId);
            kp.ShouldNotBeNull();
            kp.Title.ShouldBe(newTitle);
        }
        else 
        {
            var ex = Should.Throw<Exception>(() => controller.UpdateKeypoint(tourId, keypointId, updated));

            if(expectedStatus == 400)
            {
                ex.ShouldBeOfType<ArgumentException>();
            }
            else if (expectedStatus == 403)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }
            else if(expectedStatus == 404)
            {
                ex.ShouldBeOfType<NotFoundException>();
            }
        }
    }

    [Theory]
    [InlineData("-1", -5, -1, 200)] // Valid
    [InlineData("-2", -15, -1, 403)] // Invalid, unauthorized
    [InlineData("-1", -5, 77, 404)] // Invalid, keypoint not found
    [InlineData("-1", -77, -1, 404)] // Invalid, tour not found
    public void Deletes_keypoint(string authorId, long tourId, long keypointId, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        if (expectedStatus == 200)
        {
            // Act
            controller.DeleteKeypoint(tourId, keypointId);

            // Assert - database
            var storedTour = dbContext.Tour.First(t => t.Id == tourId);
            var kp = storedTour.Keypoints.FirstOrDefault(k => k.Id == keypointId);
            kp.ShouldBeNull();
        }
        else
        {
            var ex = Should.Throw<Exception>(() => controller.DeleteKeypoint(tourId, keypointId));

            if(expectedStatus == 403)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }
            else if(expectedStatus == 404)
            {
                ex.ShouldBeOfType<NotFoundException>();
            }
        }
    }

    [Theory]
    [InlineData("-1", -5, -1, 200)] // Valid
    [InlineData("-2", -5, -2, 403)] // Invalid, unathorizted
    [InlineData("-1", -5, -1, 403)] // Invalid, already exists
    [InlineData("-1", -5, -9999999, 404)] // Invalid, equipment doesn't exist
    [InlineData("-1", -9999999, -1, 404)] // Invalid, tour doesn't exist
    public void Adds_equipment_to_tour(string authorId, long tourId, long equipmentId, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();


        if(expectedStatus == 200)
        {
            // Act
            var response = controller.AddEquipment(tourId, equipmentId);


            // Assert – Database
            var stored = dbContext.Tour
                .Include(t => t.Equipment)
                .First(t => t.Id == tourId);
            stored.Equipment.ShouldContain(e => e.Id == equipmentId);
        }
        else
        {
            var ex = Should.Throw<Exception>(() => controller.AddEquipment(tourId, equipmentId));

            if (expectedStatus == 403)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }
            else if (expectedStatus == 404)
            {
                ex.ShouldBeOfType<NotFoundException>();
            }
        }
    }

    [Theory]
    [InlineData("-1", -5, -1, 200)]        // Valid
    [InlineData("-1", -5, -1, 403)]        // Invalid, equipment not on tour
    [InlineData("-1", -5, -9999999, 404)]  // Invalid, equipment doesn't exist
    [InlineData("-1", -9999999, -1, 404)]  // Invalid, tour doesn't exist
    public void Removes_equipment_from_tour(string authorId, long tourId, long equipmentId, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        if (expectedStatus == 200)
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

            // Act
            var response = controller.RemoveEquipment(tourId, equipmentId);

            // Assert – Database
            var stored = dbContext.Tour
                .Include(t => t.Equipment)
                .First(t => t.Id == tourId);

            stored.Equipment.ShouldNotContain(e => e.Id == equipmentId);
        }
        else
        {
            var ex = Should.Throw<Exception>(() => controller.RemoveEquipment(tourId, equipmentId));

            if (expectedStatus == 403)
            {
                ex.ShouldBeOfType<InvalidOperationException>(); // trying to remove something not assigned
            }
            else if (expectedStatus == 404)
            {
                ex.ShouldBeOfType<NotFoundException>();
            }
        }
    }

}
