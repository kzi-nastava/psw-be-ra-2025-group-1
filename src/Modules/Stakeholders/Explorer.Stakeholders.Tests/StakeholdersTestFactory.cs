using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        services.Remove(descriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        return services;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using var scope = BuildServiceProvider(services).CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILogger<StakeholdersTestFactory>>();

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
            logger.LogInformation("=== Starting database initialization ===");
            
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

            logger.LogInformation("=== Loading Tours test data ===");
            
            var toursTestDataPath = Path.Combine(".", "..", "..", "..", "..", "..", "Tours", "Explorer.Tours.Tests", "TestData");
            logger.LogInformation($"Looking for Tours test data at: {Path.GetFullPath(toursTestDataPath)}");
            
            if (Directory.Exists(toursTestDataPath))
            {
                var toursScriptFiles = Directory.GetFiles(toursTestDataPath, "*.sql");
                Array.Sort(toursScriptFiles);
                logger.LogInformation($"Found {toursScriptFiles.Length} Tours SQL scripts");
                
                foreach (var scriptFile in toursScriptFiles)
                {
                    try
                    {
                        var script = File.ReadAllText(scriptFile);
                        toursContext.Database.ExecuteSqlRaw(script);
                        logger.LogInformation($"✅ Executed Tours script: {Path.GetFileName(scriptFile)}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"❌ Failed to execute Tours script: {Path.GetFileName(scriptFile)}");
                    }
                }
            }
            else
            {
                logger.LogWarning($"⚠️ Tours test data directory not found at: {Path.GetFullPath(toursTestDataPath)}");
            }

            logger.LogInformation("=== Loading Stakeholders test data ===");
            
            // Pa onda učitaj Stakeholders test podatke (Problem entiteti koji referenciraju Tour)
            var stakeholdersTestDataPath = Path.Combine(".", "..", "..", "..", "TestData");
            logger.LogInformation($"Looking for Stakeholders test data at: {Path.GetFullPath(stakeholdersTestDataPath)}");
            
            if (Directory.Exists(stakeholdersTestDataPath))
            {
                // Load both .sql and .pgsql files
                var stakeholdersScriptFiles = Directory.GetFiles(stakeholdersTestDataPath, "*.sql")
                    .Concat(Directory.GetFiles(stakeholdersTestDataPath, "*.pgsql"))
                    .ToArray();
                Array.Sort(stakeholdersScriptFiles);
                logger.LogInformation($"Found {stakeholdersScriptFiles.Length} Stakeholders SQL/PGSQL scripts");
                
                foreach (var scriptFile in stakeholdersScriptFiles)
                {
                    try
                    {
                        var script = File.ReadAllText(scriptFile);
                        stakeholdersContext.Database.ExecuteSqlRaw(script);
                        logger.LogInformation($"✅ Executed Stakeholders script: {Path.GetFileName(scriptFile)}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"❌ Failed to execute Stakeholders script: {Path.GetFileName(scriptFile)}");
                    }
                }
            }
            else
            {
                logger.LogWarning($"⚠️ Stakeholders test data directory not found at: {Path.GetFullPath(stakeholdersTestDataPath)}");
            }
            
            logger.LogInformation("=== Database initialization completed ===");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization. Error: {Message}", ex.Message);
            throw;
        }
    }
}