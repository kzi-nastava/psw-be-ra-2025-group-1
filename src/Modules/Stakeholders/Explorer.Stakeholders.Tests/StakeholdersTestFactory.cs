using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EncounterContext>));
        if (descriptor != null) services.Remove(descriptor);
        services.AddDbContext<EncounterContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        if (descriptor != null) services.Remove(descriptor);
        services.AddDbContext<ToursContext>(SetupTestContext());

        return services;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // Create Encounters and Tours schema tables (needed by ProfileViewService)
        builder.ConfigureServices(services =>
        {
            using var scope = ReplaceNeededDbContexts(services).BuildServiceProvider().CreateScope();

            foreach (var contextType in new[] { typeof(EncounterContext), typeof(ToursContext) })
            {
                var ctx = (DbContext)scope.ServiceProvider.GetRequiredService(contextType);
                try
                {
                    ctx.Database.EnsureCreated();
                    var creator = ctx.Database.GetService<IRelationalDatabaseCreator>();
                    creator.CreateTables();
                }
                catch (Exception)
                {
                    // Tables might already exist
                }
            }
        });
    }
}
