using Explorer.BuildingBlocks.Tests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.ProjectAutopsy.Infrastructure.Database;

namespace Explorer.ProjectAutopsy.Tests;

public class ProjectAutopsyTestFactory : WebApplicationFactory<API.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProjectAutopsyContext>();

            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        });
    }
}
