using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class WalletCommandTests : BasePaymentsIntegrationTest
    {
        public WalletCommandTests(PaymentsTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var touristId = -24; // New tourist without wallet

            // Act
            var result = service.Create(touristId);

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.TouristId.ShouldBe(touristId);
            result.Balance.ShouldBe(0);

            // Assert - Database
            var storedEntity = dbContext.Set<Explorer.Payments.Core.Domain.Wallet>().FirstOrDefault(w => w.Id == result.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.TouristId.ShouldBe(touristId);
        }

        [Fact]
        public void Updates_balance()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var walletId = 1;
            var newBalance = 500.0;

            // Act
            var result = service.UpdateBalance(walletId, newBalance);

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(walletId);
            result.Balance.ShouldBe((decimal)newBalance);

            // Assert - Database
            var storedEntity = dbContext.Set<Explorer.Payments.Core.Domain.Wallet>().FirstOrDefault(w => w.Id == walletId);
            storedEntity.ShouldNotBeNull();
            storedEntity.Balance.ShouldBe(newBalance);
        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var walletId = 2;

            // Act
            service.Delete(walletId);

            // Assert - Database
            var storedEntity = dbContext.Set<Explorer.Payments.Core.Domain.Wallet>().FirstOrDefault(w => w.Id == walletId);
            storedEntity.ShouldBeNull();
        }

        [Fact]
        public void Gets_by_tourist_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var touristId = -24;

            // Act
            var result = service.GetByTouristId(touristId);

            // Assert
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(touristId);
            result.Balance.ShouldBe(100);
        }
    }
}
