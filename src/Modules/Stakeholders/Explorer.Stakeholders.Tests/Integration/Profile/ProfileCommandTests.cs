using Explorer.API.Controllers;
using Explorer.API.Views.ProfileView;
using Explorer.API.Views.ProfileView.Dtos;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Tests.Integration.Profile
{
    [Collection("Sequential")]
    public class ProfileCommandTests : BaseStakeholdersIntegrationTest
    {
        public ProfileCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();

            var controller = CreateController(scope);

            // Creating a fake identity because the method requires the user to be logged in
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", "-11"),
                new Claim("personId", "-11"),
                new Claim(ClaimTypes.Role, "tourist")
            }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var updatedEntity = new PersonDto
            {
                Id = -11,
                UserId = -11,
                Name = "Perica",
                Surname = "Peric",
                Email = "perica@gmail.com",
                ProfileImageUrl = "",
                Biography = "Volim da pecam",
                Quote = "Nauci coveka da peca",        
            };

            // Act
            var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as PersonDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(updatedEntity.Id);
            result.UserId.ShouldBe(updatedEntity.UserId);
            result.Name.ShouldBe(updatedEntity.Name);
            result.Surname.ShouldBe(updatedEntity.Surname);
            result.Email.ShouldBe(updatedEntity.Email);
            result.ProfileImageUrl.ShouldBe(updatedEntity.ProfileImageUrl);
            result.Biography.ShouldBe(updatedEntity.Biography);
            result.Quote.ShouldBe(updatedEntity.Quote);


            // Assert - Database
            var storedEntity = dbContext.People.FirstOrDefault(p => p.Id == updatedEntity.Id);
            storedEntity.ShouldNotBeNull();

            storedEntity.Id.ShouldBe(updatedEntity.Id);
            storedEntity.UserId.ShouldBe(updatedEntity.UserId);
            storedEntity.Name.ShouldBe(updatedEntity.Name);
            storedEntity.Surname.ShouldBe(updatedEntity.Surname);
            storedEntity.Email.ShouldBe(updatedEntity.Email);
            storedEntity.ProfileImageUrl.ShouldBe(updatedEntity.ProfileImageUrl);
            storedEntity.Biography.ShouldBe(updatedEntity.Biography);
            storedEntity.Quote.ShouldBe(updatedEntity.Quote);
        }

        [Fact]
        public void Update_fails_when_user_not_logged_in()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // No one is logged in
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var personEntity = new PersonDto
            {
                Id = -12,
                UserId = -12,
                Name = "Lena",
                Surname = "Lenić",
                Email = "autor2@gmail.com",
                ProfileImageUrl = null,
                Biography = null,
                Quote = null,
            };

            // Act & Assert
            Should.Throw<UnauthorizedAccessException>(() => controller.Update(personEntity));
        }

        [Fact]
        public void Update_fails_unauthorized_user()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();

            var controller = CreateController(scope);

            // Creating a fake identity because the method requires the user to be logged in
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", "-11"),
                new Claim("personId", "-11"),
                new Claim(ClaimTypes.Role, "tourist")
             }, "TestAuthentication");
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var updatedEntity = new PersonDto
            {
                Id = -1,
                UserId = -1,
                Name = "Perica",
                Surname = "Peric",
                Email = "perica@gmail.com",
                ProfileImageUrl = "",
                Biography = "Volim da pecam",
                Quote = "Nauci coveka da peca",
            };

            // Act
            var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as PersonDto;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(-11);
            result.UserId.ShouldBe(-11);
            result.Name.ShouldBe(updatedEntity.Name);
            result.Surname.ShouldBe(updatedEntity.Surname);
            result.Email.ShouldBe(updatedEntity.Email);
            result.ProfileImageUrl.ShouldBe(updatedEntity.ProfileImageUrl);
            result.Biography.ShouldBe(updatedEntity.Biography);
            result.Quote.ShouldBe(updatedEntity.Quote);
        }

        // WARNING: BANGAV TEST!!1!!!1!
        [Fact]
        public void Gets()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var existingEntity = new PersonDto
            {
                Id = -12,
                UserId = -12,
                Name = "Lena",
                Surname = "Lenić",
                Email = "autor2@gmail.com",
                ProfileImageUrl = null,
                Biography = null,
                Quote = null,
            };

            // Act
            var result = ((ObjectResult)controller.Get(existingEntity.Id).Result)?.Value as TouristProfileDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(existingEntity.Id);
            result.UserId.ShouldBe(existingEntity.UserId);
            result.Name.ShouldBe(existingEntity.Name);
            result.Surname.ShouldBe(existingEntity.Surname);
            //result.Email.ShouldBe(existingEntity.Email);
            result.ProfileImageUrl.ShouldBe(existingEntity.ProfileImageUrl);
            result.Biography.ShouldBe(existingEntity.Biography);
            result.Quote.ShouldBe(existingEntity.Quote);
        }

        [Fact]
        public void Get_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Act & Assert
            Should.Throw<NotFoundException>(() => controller.Get(-1000));
        }

        private static ProfileController CreateController(IServiceScope scope)
        {
            return new ProfileController(scope.ServiceProvider.GetRequiredService<IPersonService>(), scope.ServiceProvider.GetRequiredService<IUserManagementService>(), scope.ServiceProvider.GetRequiredService<ProfileViewService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }

    }
}
