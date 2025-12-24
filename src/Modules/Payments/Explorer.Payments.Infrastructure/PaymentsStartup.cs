using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases; // AutoMapper profil
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database.Repositories;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<Explorer.BuildingBlocks.Core.Services.ITourPurchaseTokenChecker,
                Explorer.Payments.Core.Services.TourPurchaseTokenChecker>();

        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>();
            services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();
            services.AddDbContext<PaymentsContext>(options =>
                options.UseNpgsql("Host=localhost;Database=payments;Username=postgres;Password=postgres"));

        }
    }
}
