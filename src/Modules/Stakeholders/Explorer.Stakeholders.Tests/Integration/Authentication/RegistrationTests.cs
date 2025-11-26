using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Authentication;

[Collection("Sequential")]
public class RegistrationTests : BaseStakeholdersIntegrationTest
{
    public RegistrationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Successfully_registers_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "turistaA@gmail.com",
            Email = "turistaA@gmail.com",
            Password = "turistaA",
            Name = "Žika",
            Surname = "Žikić"
        };

        // Act
        var authenticationResponse = ((ObjectResult)controller.RegisterTourist(account).Result).Value as AuthenticationTokensDto;

        // Assert - Response
        authenticationResponse.ShouldNotBeNull();
        authenticationResponse.Id.ShouldNotBe(0);
        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
        personId.Value.ShouldNotBe("0");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedAccount = dbContext.Users.FirstOrDefault(u => u.Username == account.Email);
        storedAccount.ShouldNotBeNull();
        storedAccount.Role.ShouldBe(UserRole.Tourist);
        var storedPerson = dbContext.People.FirstOrDefault(i => i.Email == account.Email);
        storedPerson.ShouldNotBeNull();
        storedPerson.UserId.ShouldBe(storedAccount.Id);
        storedPerson.Name.ShouldBe(account.Name);
    }

    [Fact]
    public void Successfully_registers_admins()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        var existingAdmin = new CredentialsDto
        {
            Username = "admin@gmail.com",
            Password = "admin",
        };
        var loginResponse = ((ObjectResult)controller.Login(existingAdmin).Result).Value as AuthenticationTokensDto;

        // Dekodiramo Token da dobijemo userId
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(loginResponse.AccessToken);
        var userId = long.Parse(decodedToken.Claims.First(c => c.Type == "id").Value);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateAdminClaimsPrincipal(userId, existingAdmin.Username)
            }
        };

        var newAdmin = new AccountRegistrationDto
        {
            Username = "newAdmin@gmail.com",
            Email = "newAdmin@gmail.com",
            Password = "admin456",
            Name = "New",
            Surname = "Admin"
        };

        // Act
        var result = ((ObjectResult)controller.RegisterAdmin(newAdmin).Result).Value as AuthenticationTokensDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);

        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
        personId.Value.ShouldNotBe("0");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedAccount = dbContext.Users.FirstOrDefault(u => u.Username == newAdmin.Username);
        storedAccount.ShouldNotBeNull();
        storedAccount.Role.ShouldBe(UserRole.Administrator);

        var storedPerson = dbContext.People.FirstOrDefault(p => p.Email == newAdmin.Email);
        storedPerson.ShouldNotBeNull();
        storedPerson.UserId.ShouldBe(storedAccount.Id);
        storedPerson.Name.ShouldBe(newAdmin.Name);
    }

    [Fact]
    public void Successfully_registers_authors()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        var existingAdmin = new CredentialsDto
        {
            Username = "admin@gmail.com",
            Password = "admin",
        };
        var loginResponse = ((ObjectResult)controller.Login(existingAdmin).Result).Value as AuthenticationTokensDto;

        // Dekodiramo Token da dobijemo userId
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(loginResponse.AccessToken);
        var userId = long.Parse(decodedToken.Claims.First(c => c.Type == "id").Value);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateAdminClaimsPrincipal(userId, existingAdmin.Username) 
            }
        };

        var newAuthor = new AccountRegistrationDto
        {
            Username = "autorA@gmail.com",
            Email = "autorA@gmail.com",
            Password = "autor645",
            Name = "Novi",
            Surname = "Autor"
        };

        // Act
        var result = ((ObjectResult)controller.RegisterAuthor(newAuthor).Result).Value as AuthenticationTokensDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);

        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
        personId.Value.ShouldNotBe("0");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedAccount = dbContext.Users.FirstOrDefault(u => u.Username == newAuthor.Username);
        storedAccount.ShouldNotBeNull();
        storedAccount.Role.ShouldBe(UserRole.Author);

        var storedPerson = dbContext.People.FirstOrDefault(p => p.Email == newAuthor.Email);
        storedPerson.ShouldNotBeNull();
        storedPerson.UserId.ShouldBe(storedAccount.Id);
        storedPerson.Name.ShouldBe(newAuthor.Name);
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

    private static AuthenticationController CreateController(IServiceScope scope)
    {
        return new AuthenticationController(
            scope.ServiceProvider.GetRequiredService<IAuthenticationService>(),
            scope.ServiceProvider.GetRequiredService<IUserRepository>());
    }
}