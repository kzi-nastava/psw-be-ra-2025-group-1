using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Explorer.Payments.Tests
{
    public class PaymentsTestFactory:BaseTestFactory<PaymentsContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            // Replace PaymentsContext
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
            services.Remove(descriptor!);
            services.AddDbContext<PaymentsContext>(SetupTestContext());

            // Replace ToursContext - needed for checkout tests that reference tours
            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<ToursContext>(SetupTestContext());

            return services;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Create tours tables BEFORE base runs SQL scripts (f-tours.sql needs tours schema)
            Environment.SetEnvironmentVariable("RUNNING_TESTS", "true");

            builder.ConfigureServices(services =>
            {
                using var scope = ReplaceNeededDbContexts(services).BuildServiceProvider().CreateScope();
                var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

                try
                {
                    toursContext.Database.EnsureCreated();
                    var databaseCreator = toursContext.Database.GetService<IRelationalDatabaseCreator>();
                    databaseCreator.CreateTables();
                }
                catch (Exception)
                {
                    // Tables might already exist
                }
            });

            base.ConfigureWebHost(builder);
        }
    }
}
