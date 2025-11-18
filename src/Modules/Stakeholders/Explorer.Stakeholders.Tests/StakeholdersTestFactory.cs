using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        // Ukloni postojeći DbContext
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        if (descriptor != null)
            services.Remove(descriptor);

        // Dodaj testni DbContext sa migracijama
        services.AddDbContext<StakeholdersContext>(options =>
            options.UseNpgsql(
                "Host=localhost;Database=stakeholders_test;Username=postgres;Password=root",
                o => o.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")
            ));

        return services;
    }
}
