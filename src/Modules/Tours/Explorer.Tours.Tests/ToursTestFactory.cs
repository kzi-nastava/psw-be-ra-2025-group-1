using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        services.Remove(descriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        // Add PaymentsContext to ensure payments schema tables are created
        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        services.AddDbContext<PaymentsContext>(SetupTestContext());

        return services;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        
        // Ensure Payments schema tables are created and initialized
        builder.ConfigureServices(services =>
        {
            using var scope = BuildServiceProvider(services).CreateScope();
            var paymentsContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ToursTestFactory>>();
            
            try
            {
                // Create the payments schema and tables
                paymentsContext.Database.EnsureCreated();
                var databaseCreator = paymentsContext.Database.GetService<IRelationalDatabaseCreator>();
                databaseCreator.CreateTables();
                
                logger.LogInformation("Payments schema tables created successfully");
            }
            catch (Exception ex)
            {
                // Tables might already exist - this is expected
                logger.LogWarning(ex, "Could not create Payments tables (they might already exist): {Message}", ex.Message);
            }
        });
    }

    private ServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return ReplaceNeededDbContexts(services).BuildServiceProvider();
    }
}
