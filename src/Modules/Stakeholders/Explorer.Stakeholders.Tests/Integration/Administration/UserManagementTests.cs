using Explorer.API.Controllers;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class UserManagementTests : BaseStakeholdersIntegrationTest
    {
        public UserManagementTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Successfully_get_users()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var controller = CreateController(scope);
            var authController = CreateAuthController(scope);

            var existingAdmin = new CredentialsDto
            {
                Username = "admin@gmail.com",
                Password = "admin",
            };
            var loginResponse = ((ObjectResult)authController.Login(existingAdmin).Result).Value as AuthenticationTokensDto;
            var userId = loginResponse.Id;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateAdminClaimsPrincipal(userId, existingAdmin.Username)
                }
            };

            // Act
            var result = ((ObjectResult)controller.GetAll().Result).Value as List<AccountDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);

            foreach (var account in result)
            {
                account.Id.ShouldNotBe(0);
                account.Username.ShouldNotBeNullOrEmpty();
                account.Email.ShouldNotBeNullOrEmpty();
                account.Name.ShouldNotBeNullOrEmpty();
                account.Surname.ShouldNotBeNullOrEmpty();
                Enum.IsDefined(typeof(AccountRole), account.Role).ShouldBeTrue();
                account.IsActive.ShouldBeOfType<bool>();
            }

            // Provera da li broj korisnika odgovara u bazi
            dbContext.ChangeTracker.Clear();
            var usersInDb = dbContext.Users.Count();
            result.Count.ShouldBe(usersInDb);
        }

        [Fact]
        public void Successfully_block_users()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var controller = CreateController(scope);
            var authController = CreateAuthController(scope);

            var existingAdmin = new CredentialsDto
            {
                Username = "admin@gmail.com",
                Password = "admin",
            };
            var loginResponse = ((ObjectResult)authController.Login(existingAdmin).Result).Value as AuthenticationTokensDto;
            var userId = loginResponse.Id;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateAdminClaimsPrincipal(userId, existingAdmin.Username)
                }
            };

            var users = dbContext.Users.ToList();

            foreach (var user in users)
            {
                if (user.Username.Equals(existingAdmin.Username)) { continue; }
                user.IsActive.ShouldBeTrue();

                // Act
                var result = controller.BlockUser(user.Id);

                // Assert
                result.ShouldBeOfType<NoContentResult>();

                dbContext.ChangeTracker.Clear();
                var blockedUser = dbContext.Users.Find(user.Id);
                blockedUser.ShouldNotBeNull();
                blockedUser.IsActive.ShouldBeFalse();
            }
        }

        // Helper metoda ClaimsPrincipal
        private ClaimsPrincipal CreateAdminClaimsPrincipal(long userId, string username)
        {
            var claims = new List<Claim>
            {
                new Claim("id", userId.ToString()),
                new Claim("username", username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            return new ClaimsPrincipal(identity);
        }

        private static UserManagementController CreateController(IServiceScope scope)
        {
            return new UserManagementController(scope.ServiceProvider.GetRequiredService<IUserManagementService>());
        }

        private static AuthenticationController CreateAuthController(IServiceScope scope)
        {
            return new AuthenticationController(
                scope.ServiceProvider.GetRequiredService<IAuthenticationService>(),
                scope.ServiceProvider.GetRequiredService<IUserRepository>()
                );
        }
    }
}
