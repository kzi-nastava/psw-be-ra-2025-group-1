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
            result.Balance.ShouldBe(10000000);

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
            var walletDto = new WalletDto
            {
                Id = -1,
                TouristId = -21,
                Balance = 500.0
            };

            // Act
            var result = service.UpdateBalance(walletDto.Id, walletDto);

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(walletDto.Id);
            result.Balance.ShouldBe(walletDto.Balance);

            // Assert - Database
            var storedEntity = dbContext.Set<Explorer.Payments.Core.Domain.Wallet>().FirstOrDefault(w => w.Id == walletDto.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Balance.ShouldBe(walletDto.Balance);
        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            
            // Create a wallet specifically for deletion test
            var touristId = -25; // Use unique tourist ID
            var createdWallet = service.Create(touristId);
            var walletId = createdWallet.Id;

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
            var touristId = -21;
            // Act
            var result = service.GetByTouristId(touristId);

            // Assert
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(touristId);
        }
    }
}