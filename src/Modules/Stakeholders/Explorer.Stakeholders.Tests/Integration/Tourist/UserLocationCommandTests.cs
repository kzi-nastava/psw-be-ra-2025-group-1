using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class UserLocationCommandTests : BaseStakeholdersIntegrationTest
    {
        public UserLocationCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var newEntity = new UserLocationDto
            {
                Latitude = 45.0,
                Longitude = 20.0,
                Timestamp = DateTime.UtcNow
            };

            // Logging in fake user
            var identity = new ClaimsIdentity(new[]
    {
                new Claim("id", "-21"),
                new Claim("personId", "-21"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as UserLocationDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(-21); // From Claims
            result.Latitude.ShouldBe(newEntity.Latitude);
            result.Longitude.ShouldBe(newEntity.Longitude);

            // Assert - Database
            var storedEntity = dbContext.UserLocations.FirstOrDefault(i => i.UserId == -21);
            storedEntity.ShouldNotBeNull();
            storedEntity.Latitude.ShouldBe(newEntity.Latitude);
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var updatedEntity = new UserLocationDto
            {
                Id = -1, // Match the ID from e-userlocations.sql for user -21
                UserId = -21, 
                Latitude = 46.0,
                Longitude = 21.0,
                Timestamp = DateTime.UtcNow
            };

            // Logging in fake user
            var identity = new ClaimsIdentity(new[]
    {
                new Claim("id", "-21"),
                new Claim("personId", "-21"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            // Act
            var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as UserLocationDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(-21);
            result.Latitude.ShouldBe(updatedEntity.Latitude);
            result.Longitude.ShouldBe(updatedEntity.Longitude);

            // Assert - Database
            var storedEntity = dbContext.UserLocations.FirstOrDefault(i => i.UserId == -21);
            storedEntity.ShouldNotBeNull();
            storedEntity.Latitude.ShouldBe(updatedEntity.Latitude);
        }

        [Fact]
        public void Gets()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Logging in fake user
            var identity = new ClaimsIdentity(new[]
    {
                new Claim("id", "-21"),
                new Claim("personId", "-21"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            // Act
            var result = ((ObjectResult)controller.Get().Result)?.Value as UserLocationDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(-21);
            result.Latitude.ShouldBe(45); // From e-userlocations.sql
        }

        private static UserLocationController CreateController(IServiceScope scope) // Iz nekog razloga nije radio login sa ovim
        {
            var controller = new UserLocationController(scope.ServiceProvider.GetRequiredService<IUserLocationService>());
            var claims = new[]
            {
                new Claim("id", "-21"),
                new Claim(ClaimTypes.NameIdentifier, "-21"),
                new Claim(ClaimTypes.Role, "tourist")
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
}
