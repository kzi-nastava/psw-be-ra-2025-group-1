using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases;
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
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<Explorer.Payments.API.Public.Author.ICouponService, CouponService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<Explorer.Payments.API.Public.IBundleService, BundleService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<ISalePublicService, SalePublicService>();
            // Adapter za Sale servic za Tours modul
            services.AddScoped<Explorer.BuildingBlocks.Core.Services.ISaleService, Explorer.Payments.Core.Services.SaleServiceAdapter>();

            services.AddScoped<Explorer.BuildingBlocks.Core.Services.ITourPurchaseTokenChecker,
                Explorer.Payments.Core.Services.TourPurchaseTokenChecker>();
            services.AddScoped<Explorer.BuildingBlocks.Core.Services.ISaleService,
                Explorer.Payments.Core.Services.SaleServiceAdapter>();
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>();
            services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();
            services.AddScoped<ITourPurchaseRepository, TourPurchaseDbRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICouponRedemptionRepository, CouponRedemptionRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IBundleRepository, BundleRepository>();
            services.AddScoped<IBundlePurchaseRepository, BundlePurchaseRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICouponRedemptionRepository, CouponRedemptionRepository>();
            
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<PaymentsContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
        }
    }
}
