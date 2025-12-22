using Explorer.Payments.Core.Mappers; // AutoMapper profil
using Explorer.Payments.Infrastructure.Database;
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
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddDbContext<PaymentsContext>(options =>
                options.UseNpgsql("Host=localhost;Database=payments;Username=postgres;Password=postgres"));

        }
    }
}
