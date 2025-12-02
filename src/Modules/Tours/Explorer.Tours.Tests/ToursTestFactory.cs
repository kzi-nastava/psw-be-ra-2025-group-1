using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        services.Remove(descriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        return services;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using var scope = BuildServiceProvider(services).CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILogger<ToursTestFactory>>();

            var stakeholdersDb = scopedServices.GetRequiredService<StakeholdersContext>();
            var toursDb = scopedServices.GetRequiredService<ToursContext>();

            InitializeDatabase(stakeholdersDb, toursDb, logger);
        });
    }

    private ServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return ReplaceNeededDbContexts(services).BuildServiceProvider();
    }

    private void InitializeDatabase(StakeholdersContext stakeholdersContext, ToursContext toursContext, ILogger logger)
    {
        try
        {
            // Obriši i kreiraj bazu samo jednom
            stakeholdersContext.Database.EnsureDeleted();
            stakeholdersContext.Database.EnsureCreated();
            
            // Kreiraj tabele za Tours kontekst (bez brisanja baze)
            var toursDatabaseCreator = toursContext.Database.GetService<IRelationalDatabaseCreator>();
            try
            {
                toursDatabaseCreator.CreateTables();
                logger.LogInformation("Created Tours tables");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "CreateTables for ToursContext threw an exception (might already exist)");
            }

            // Prvo učitaj Stakeholders test podatke (Problem entiteti)
            var stakeholdersTestDataPath = Path.Combine(".", "..", "..", "..", "..", "Stakeholders", "Explorer.Stakeholders.Tests", "TestData");
            if (Directory.Exists(stakeholdersTestDataPath))
            {
                var stakeholdersScriptFiles = Directory.GetFiles(stakeholdersTestDataPath, "*.sql");
                Array.Sort(stakeholdersScriptFiles);
                foreach (var scriptFile in stakeholdersScriptFiles)
                {
                    try
                    {
                        var script = File.ReadAllText(scriptFile);
                        stakeholdersContext.Database.ExecuteSqlRaw(script);
                        logger.LogInformation($"Executed Stakeholders script: {Path.GetFileName(scriptFile)}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"Failed to execute Stakeholders script: {Path.GetFileName(scriptFile)}");
                    }
                }
            }

            // Pa onda učitaj Tours test podatke (ProblemMessage entiteti)
            var toursTestDataPath = Path.Combine(".", "..", "..", "..", "TestData");
            if (Directory.Exists(toursTestDataPath))
            {
                var toursScriptFiles = Directory.GetFiles(toursTestDataPath, "*.sql");
                Array.Sort(toursScriptFiles);
                foreach (var scriptFile in toursScriptFiles)
                {
                    try
                    {
                        var script = File.ReadAllText(scriptFile);
                        toursContext.Database.ExecuteSqlRaw(script);
                        logger.LogInformation($"Executed Tours script: {Path.GetFileName(scriptFile)}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"Failed to execute Tours script: {Path.GetFileName(scriptFile)}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization. Error: {Message}", ex.Message);
            throw;
        }
    }
}
