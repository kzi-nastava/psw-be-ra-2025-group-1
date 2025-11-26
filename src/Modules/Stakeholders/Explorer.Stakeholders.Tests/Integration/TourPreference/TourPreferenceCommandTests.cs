using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Tests.Integration.TourPreference
{
    [Collection("Sequential")]
    public class TourPreferenceCommandTests : BaseStakeholdersIntegrationTest
    {
        public TourPreferenceCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            // Creating a fake identity because the method requires the user to be logged in
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", "-21"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var updatedEntity = new TourPreferenceDto
            {
                Id = -21,
                UserId = -21,
                BicycleRating = 3,
                CarRating = 0,
                BoatRating = null,
                WalkRating = 2,
                Difficulty = 4.5,
                PreferedTags = new List<string> { "history", "nature", "food" },
            };
            // Act
            var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TourPreferenceDto;
            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(updatedEntity.Id);
            result.UserId.ShouldBe(updatedEntity.UserId);
            result.BicycleRating.ShouldBe(updatedEntity.BicycleRating);
            result.CarRating.ShouldBe(updatedEntity.CarRating);
            result.BoatRating.ShouldBe(updatedEntity.BoatRating);
            result.WalkRating.ShouldBe(updatedEntity.WalkRating);
            result.Difficulty.ShouldBe(updatedEntity.Difficulty);
            result.PreferedTags.ShouldBeEquivalentTo(updatedEntity.PreferedTags);
            // Assert - Database
            var entityInDb = dbContext.TourPreferences.Find(-21L);
            entityInDb.ShouldNotBeNull();
            entityInDb.UserId.ShouldBe(updatedEntity.UserId);
            entityInDb.BicycleRating.ShouldBe(updatedEntity.BicycleRating);
            entityInDb.CarRating.ShouldBe(updatedEntity.CarRating);
            entityInDb.BoatRating.ShouldBe(updatedEntity.BoatRating);
            entityInDb.WalkRating.ShouldBe(updatedEntity.WalkRating);
            entityInDb.Difficulty.ShouldBe(updatedEntity.Difficulty);
            entityInDb.PreferedTags.ShouldBeEquivalentTo(updatedEntity.PreferedTags);
        }

        [Fact]
        public void Update_fails_when_user_not_logged_in()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new TourPreferenceDto
            {
                Id = -21,
                UserId = -21,
                BicycleRating = 3,
                CarRating = 0,
                BoatRating = null,
                WalkRating = 2,
                Difficulty = 4.5,
                PreferedTags = new List<string> { "history", "nature", "food" },
            };
            // Act
            Should.Throw<UnauthorizedAccessException>(() =>
            {
                var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TourPreferenceDto;
            });
        }

        [Fact]
        public void Update_fails_when_updating_other_user_preference()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            // Creating a fake identity because the method requires the user to be logged in
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", "-12"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
            var updatedEntity = new TourPreferenceDto
            {
                Id = -21,
                UserId = -21,
                BicycleRating = 3,
                CarRating = 0,
                BoatRating = null,
                WalkRating = 2,
                Difficulty = 4.5,
                PreferedTags = new List<string> { "history", "nature", "food" },
            };
            // Act & Assert
            Should.Throw<UnauthorizedAccessException>(() =>
            {
                var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TourPreferenceDto;
            });
        }

        [Fact]
        public void Gets()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            // Creating a fake identity because the method requires the user to be logged in
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", "-23"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
            // Act
            var result = ((ObjectResult)controller.Get().Result)?.Value as TourPreferenceDto;
            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-23);
            result.UserId.ShouldBe(-23);
            result.Difficulty.ShouldBe(null);
            result.WalkRating.ShouldBe(0);
            result.BicycleRating.ShouldBe(null);
            result.CarRating.ShouldBe(3);
            result.BoatRating.ShouldBe(3);
            result.PreferedTags.ShouldBeEquivalentTo(new List<string> { "adrenaline", "adventure" });
        }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            // Creating a fake identity because the method requires the user to be logged in
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", "-22"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
            var newEntity = new TourPreferenceDto
            {
                UserId = -22,
                BicycleRating = 1,
                CarRating = 2,
                BoatRating = 3,
                WalkRating = 0,
                Difficulty = 2.5,
                PreferedTags = new List<string> { "culture", "food" },
            };
            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourPreferenceDto;
            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBeGreaterThan(0);
            result.UserId.ShouldBe(newEntity.UserId);
            result.BicycleRating.ShouldBe(newEntity.BicycleRating);
            result.CarRating.ShouldBe(newEntity.CarRating);
            result.BoatRating.ShouldBe(newEntity.BoatRating);
            result.WalkRating.ShouldBe(newEntity.WalkRating);
            result.Difficulty.ShouldBe(newEntity.Difficulty);
            result.PreferedTags.ShouldBeEquivalentTo(newEntity.PreferedTags);
        }

        [Fact(Skip = "Not implemented")]
        public void Create_fails_when_preference_already_exists()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            // Creating a fake identity because the method requires the user to be logged in
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", "-21"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
            var newEntity = new TourPreferenceDto
            {
                UserId = -21,
                BicycleRating = 1,
                CarRating = 2,
                BoatRating = 3,
                WalkRating = 0,
                Difficulty = 2.5,
                PreferedTags = new List<string> { "culture", "food" },
            };
            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
            {
                var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourPreferenceDto;
            });
        }

        private static TourPreferenceController CreateController(IServiceScope scope)
        {
            var tourPreferenceService = scope.ServiceProvider.GetRequiredService<ITourPreferenceService>();
            return new TourPreferenceController(tourPreferenceService)
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
