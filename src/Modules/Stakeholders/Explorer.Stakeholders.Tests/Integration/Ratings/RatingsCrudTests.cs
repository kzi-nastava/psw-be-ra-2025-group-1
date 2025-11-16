using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

using Explorer.Stakeholders.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.API.Dtos;
using Explorer.API.Controllers.Author;

namespace Explorer.Stakeholders.Tests.Integration.Ratings;

[Collection("Sequential")]
public class RatingsCrudTests : BaseStakeholdersIntegrationTest
{
    public RatingsCrudTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Create_rating_as_tourist_succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateAuthorControllerForUser(scope, userId: -21, role: "tourist");
        var dto = new RatingCreateDto { Score = 5, Comment = "great" };

        // Act
        var result = (OkObjectResult)controller.Create(dto).Result!;
        var created = (RatingDto)result.Value!;

        // Assert (response)
        created.UserId.ShouldBe(-21);
        created.Score.ShouldBe(5);

        // Assert (database)
        db.ChangeTracker.Clear();
        var stored = db.Ratings.FirstOrDefault(r => r.Id == created.Id);
        stored.ShouldNotBeNull();
        stored!.UserId.ShouldBe(-21);
    }

    [Fact]
    public void Update_own_rating_succeeds()
    {
        // Arrange: u seed-u -100 pripada -21
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateAuthorControllerForUser(scope, userId: -22, role: "tourist");
        var upd = new RatingUpdateDto { Score = 4, Comment = "edited" };

        // Act
        var result = (OkObjectResult)controller.Update(-100, upd).Result!;
        var updated = (RatingDto)result.Value!;

        // Assert
        updated.Score.ShouldBe(4);
        updated.Comment.ShouldBe("edited");

        db.ChangeTracker.Clear();
        var stored = db.Ratings.First(r => r.Id == -100);
        stored.Score.ShouldBe(4);
        stored.Comment.ShouldBe("edited");
    }

    [Fact]
    public void Delete_own_rating_succeeds()
    {
        // Arrange: kreiramo pa brisemo (da ne zavisimo od seed-a)
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateAuthorControllerForUser(scope, userId: -21, role: "tourist");

        var created = (RatingDto)((OkObjectResult)controller.Create(
            new RatingCreateDto { Score = 2, Comment = "temp" }).Result!).Value!;
        var id = created.Id;

        // Act
        var del = (NoContentResult)controller.Delete(id);
        del.StatusCode.ShouldBe(StatusCodes.Status204NoContent);

        // Assert
        db.ChangeTracker.Clear();
        db.Ratings.Any(r => r.Id == id).ShouldBeFalse();
    }

    [Fact]
    public void Update_someone_else_rating_forbidden()
    {
        // Arrange: -101 pripada korisniku -22, a prijavljeni je -21
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorControllerForUser(scope, userId: -21, role: "tourist");

        // Act + Assert
        Should.Throw<UnauthorizedAccessException>(() =>
        {
            // controller.Update vraca ActionResult, ali tvoj servis baca UnauthorizedAccessException
            _ = controller.Update(-100, new RatingUpdateDto { Score = 1, Comment = "nope" }).Result;
        });
    }

    private static RatingsController CreateAuthorControllerForUser(IServiceScope scope, long userId, string role)
    {
        var controller = new RatingsController(scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.API.Public.IRatingsService>());
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)      // "author" ili "tourist" – mora da se slaze sa policy
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
            }
        };
        return controller;
    }
}
