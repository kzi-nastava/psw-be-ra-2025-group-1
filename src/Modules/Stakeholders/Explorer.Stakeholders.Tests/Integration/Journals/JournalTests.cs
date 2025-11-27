using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

using Explorer.Stakeholders.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.API.Dtos;
using Explorer.API.Controllers.Author;

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
        var controller = CreateTouristControllerForUser(scope, userId: -22, role: "tourist"); // turista2
        var dto = new JournalCreateDto
        {
            Title = "Novi journal",
            Content = "Sadržaj dnevnika",
            Location = "Novi Sad"
        };

        // Act
        var result = (OkObjectResult)controller.Create(dto).Result!;
        var created = (JournalDto)result.Value!;

        // Assert (response)
        created.UserId.ShouldBe(-22);
        created.Title.ShouldBe(dto.Title);

        // Assert (database)
        db.ChangeTracker.Clear();
        var stored = db.Journals.FirstOrDefault(j => j.Id == created.Id);
        stored.ShouldNotBeNull();
        stored!.UserId.ShouldBe(-22);
        stored.Title.ShouldBe(dto.Title);
    }

    [Fact]
    public void GetMine_returns_journals_for_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristControllerForUser(scope, userId: -22, role: "tourist"); // turista2

        // Act
        var result = (OkObjectResult)controller.GetMine().Result!;
        var journals = (List<JournalDto>)result.Value!;

        // Assert
        journals.ShouldNotBeEmpty();
        journals.All(j => j.UserId == -22).ShouldBeTrue();
    }

    [Fact]
    public void Update_journal_succeeds_for_owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateTouristControllerForUser(scope, userId: -22, role: "tourist"); // turista2

        var existingJournal = db.Journals.First(j => j.UserId == -22);
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
        var controller = CreateTouristControllerForUser(scope, userId: -23, role: "tourist"); // turista3

        var journalToDelete = db.Journals.First(j => j.UserId == -23);

        // Act
        var result = controller.Delete(journalToDelete.Id);

        // Assert
        result.ShouldBeOfType<NoContentResult>();
        db.ChangeTracker.Clear();
        db.Journals.FirstOrDefault(j => j.Id == journalToDelete.Id).ShouldBeNull();
    }

    // Helper: kreira controller sa testnim turistom
    private static Explorer.API.Controllers.Tourist.JournalController CreateTouristControllerForUser(
        IServiceScope scope, long userId, string role)
    {
        var service = scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.Core.UseCases.JournalService>();
        var controller = new Explorer.API.Controllers.Tourist.JournalController(service);

        // Simuliraj ClaimsPrincipal
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
