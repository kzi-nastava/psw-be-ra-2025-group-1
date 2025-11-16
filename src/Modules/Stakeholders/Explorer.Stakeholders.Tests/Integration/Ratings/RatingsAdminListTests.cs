using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

using Explorer.Stakeholders.Tests;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Tests.Integration.Ratings;

[Collection("Sequential")]
public class RatingsAdminListTests : BaseStakeholdersIntegrationTest
{
    public RatingsAdminListTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Admin_list_returns_paged_result()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);

        // Act
        var result = (OkObjectResult)controller.AdminList(page: 1, size: 10).Result!;
        var page = result.Value!; 

        // Assert – samo sanity provera da ima neSto u seed-u
        page.ShouldNotBeNull();
    }

    private static RatingsController CreateAdminController(IServiceScope scope)
    {
        var controller = new RatingsController(scope.ServiceProvider.GetRequiredService<IRatingsService>());
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "-1"),        // admin
            new Claim(ClaimTypes.Role, "administrator")
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
