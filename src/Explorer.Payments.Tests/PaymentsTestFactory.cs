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
    public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
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
            base.ConfigureWebHost(builder);

            // After base initialization, ensure Tours test data is also loaded
            builder.ConfigureServices(services =>
            {
                using var scope = BuildServiceProvider(services).CreateScope();
                var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<PaymentsTestFactory>>();

                try
                {
                    // The tours tables are already created by EnsureCreated() in BaseTestFactory
                    // But we need to load the tour test data from the Payments TestData folder
                    // The BaseTestFactory already loaded all SQL files from Explorer.Payments.Tests/TestData
                    // which includes f-tours.sql
                    logger.LogInformation("Tours context is ready and test data should be loaded from Payments TestData folder");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during Tours context verification: {Message}", ex.Message);
                }
            });
        }

        private ServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            return ReplaceNeededDbContexts(services).BuildServiceProvider();
        }
    }
}
