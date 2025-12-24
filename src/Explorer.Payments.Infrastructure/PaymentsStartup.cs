using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases; // AutoMapper profil
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database.Repositories;
using Explorer.Payments.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Payments.Infrastructure
{
    public static class PaymentsStartup
    {
        public static IServiceCollection ConfigurePaymentsModule(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(PaymentsProfile).Assembly);
            SetupCore(services);
            SetupInfrastructure(services);

            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            // Ovde kasnije ide dependency injection za Core servise
            // npr. services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITourPurchaseTokenService, TourPurchaseTokenService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICouponRedemptionRepository, CouponRedemptionRepository>();

            services.AddScoped<Explorer.BuildingBlocks.Core.Services.ITourPurchaseTokenChecker,
                Explorer.Payments.Core.Services.TourPurchaseTokenChecker>();

        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>();
            services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<PaymentsContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
        }
    }
}
