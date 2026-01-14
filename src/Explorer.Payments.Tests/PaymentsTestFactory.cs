using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                services.AddDbContext<ToursContext>(SetupTestContext());
            }

            return services;
        }

        private static void InitializeContext(DbContext context)
        {
            context.Database.EnsureCreated();
            try
            {
                var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
                databaseCreator.CreateTables();
            }
            catch (Exception)
            {
                // Tables might already exist
            }
        }
    }
}
