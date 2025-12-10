using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
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
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory)
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
        var newEntity = new TourDto { CreatorId = 1, Title = "New Tour", Description = "A new tour", Difficulty = 3, Tags = new[] { "tag1", "tag2" }, Status = TourStatusDTO.Draft, Price = 99.99 };

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
        var newEntity = new TourDto { CreatorId = 1, Title = "To Update", Description = "Will be updated", Difficulty = 2, Tags = new[] { "t1" }, Status = TourStatusDTO.Draft, Price = 50.0 };

        var created = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        var stored = dbContext.Tour.FirstOrDefault(i => i.Title == newEntity.Title);
        stored.ShouldNotBeNull();
        var id = stored.Id;

        var updatedDto = new TourDto { CreatorId = created.CreatorId, Title = "Updated Title", Description = "Updated description", Difficulty = 5, Tags = new[] { "updated" }, Status = TourStatusDTO.Published, Price = 150.0 };

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
        var newEntity = new TourDto { CreatorId = 1, Title = "To Delete", Description = "Will be deleted", Difficulty = 1, Tags = new[] { "del" }, Status = TourStatusDTO.Draft, Price = 10.0 };

        var created = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        var stored = dbContext.Tour.FirstOrDefault(i => i.Title == newEntity.Title);
        stored.ShouldNotBeNull();
        var id = stored.Id;

        // Act
        var deleteAction = controller.Delete(id);

        // Assert - Database
        var afterDelete = dbContext.Tour.FirstOrDefault(i => i.Id == id);
        afterDelete.ShouldBeNull();
    }


    [Fact]
    public void Adds_equipment_to_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var tour = new TourDto
        {
            CreatorId = 1,
            Title = "Equipment Tour",
            Description = "Tour for equipment test",
            Difficulty = 2,
            Status = TourStatusDTO.Draft,
            Price = 75.0
        };

        var created = ((ObjectResult)controller.Create(tour).Result)?.Value as TourDto;
        created.ShouldNotBeNull();
        tour.Id = created.Id;

        var equip = new Equipment("Test Backpack", "Test equipment for testing equipment binding and removal from tours");
        dbContext.Equipment.Add(equip);
        dbContext.SaveChanges();

        // Act
        var response = controller.AddEquipment(tour.Id, equip.Id);

        // Assert – Response
        response.ShouldBeOfType<OkResult>();

        // Assert – Database
        var stored = dbContext.Tour
            .Include(t => t.Equipment)
            .First(t => t.Id == tour.Id);

        stored.Equipment.ShouldContain(e => e.Id == equip.Id);
    }

}
