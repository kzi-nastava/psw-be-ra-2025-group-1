using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

using Explorer.Stakeholders.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.API.Dtos;
using Explorer.API.Controllers.Author;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Tests.Integration.Journals;

public class JournalTests : BaseStakeholdersIntegrationTest
{
    public JournalTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Create_journal_as_tourist_succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateTouristControllerForUser(scope, userId:  22, role: "tourist");
        var dto = new JournalCreateDto
        {
            Content = "Sadržaj dnevnika",
            LocationName = "Novi Sad",
            Title = "Novi journal"
        };

        // Act
        var result = controller.Create(dto).Result!;
        if (result is BadRequestObjectResult badRequest)
        {
            var errors = badRequest.Value;
            throw new Exception($"BadRequest: {errors}");
        }
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        var created = (JournalDto)okResult!.Value!;


        // Assert (response)
        created.UserId.ShouldBe(22);
        created.Title.ShouldBe(dto.Title);

        // Assert (database)
        db.ChangeTracker.Clear();
        var stored = db.Journals.FirstOrDefault(j => j.Id == created.Id);
        stored.ShouldNotBeNull();
        stored!.UserId.ShouldBe(22);
        stored.Title.ShouldBe(dto.Title);
    }

    [Fact]
    public void GetMine_returns_journals_for_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristControllerForUser(scope, userId: 22, role: "tourist");

        // Act
        var result = (OkObjectResult)controller.GetMine().Result!;
        var journals = (List<JournalDto>)result.Value!;

        // Assert
        journals.ShouldNotBeEmpty();
        journals.All(j => j.UserId == 22).ShouldBeTrue();
    }

    [Fact]
    public void Update_journal_succeeds_for_owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateTouristControllerForUser(scope, userId: 22, role: "tourist");

        var existingJournal = db.Journals.First(j => j.UserId == 22);
        var updateDto = new JournalUpdateDto
        {
            Title = "Izmenjeni naslov",
            Content = "Izmenjeni sadržaj"
        };

        // Act
        var result = (OkObjectResult)controller.Update(existingJournal.Id, updateDto).Result!;
        var updated = (JournalDto)result.Value!;

        // Assert
        updated.Title.ShouldBe(updateDto.Title);
        updated.Content.ShouldBe(updateDto.Content);
    }

    [Fact]
    public void Delete_journal_succeeds_for_owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var journal = new Journal(
            content:"test",
            userId: 23,
            title: "Test title",
            lat: 45.0,
            longit: 19.0,
            locationName: "Test location"
        );
        db.Journals.Add(journal);
        db.SaveChanges();

        var controller = CreateTouristControllerForUser(scope, userId: 23, role: "tourist");
        // Act
        var result = controller.Delete(journal.Id);

        // Assert
        result.ShouldBeOfType<NoContentResult>();
        db.ChangeTracker.Clear();
        db.Journals.FirstOrDefault(j => j.Id == journal.Id).ShouldBeNull();
    }
    
    private static Explorer.API.Controllers.Tourist.JournalController CreateTouristControllerForUser(
        IServiceScope scope, long userId, string role)
    {
        var service = scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.API.Public.IJournalService>();
        var controller = new Explorer.API.Controllers.Tourist.JournalController(service);
        
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[]
                {
                    new System.Security.Claims.Claim("id", userId.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)
                }, "test"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };

        return controller;
    }
}
