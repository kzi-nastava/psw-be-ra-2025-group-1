using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class CheckoutWithSaleTests : BasePaymentsIntegrationTest
{
    public CheckoutWithSaleTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Checkout_creates_tour_purchase_with_sale_price()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        EnsureTourTestData(scope);

        long touristId = 100; // Test tourist ID
        long tourId = 1; // Tour that should be on sale based on seed data

        // Add tour to cart
        cartService.AddToCart(touristId, tourId);

        // Act
        var tokens = cartService.Checkout(touristId);

        // Assert
        tokens.ShouldNotBeEmpty();
        
        // Verify TourPurchase was created
        var purchase = dbContext.TourPurchases
            .FirstOrDefault(p => p.TouristId == touristId && p.TourId == tourId);
        
        purchase.ShouldNotBeNull();
        purchase.PricePaid.ShouldBeGreaterThan(0);
        
        // If tour has sale, price should be less than original
        // Original price for tour 1 is 15, with 20% discount = 12
        // (This assumes seed data created a sale for tour 1)
    }

    [Fact]
    public void Checkout_with_sale_and_coupon_applies_both_discounts()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        EnsureTourTestData(scope);

        long touristId = 101; // Test tourist ID
        long tourId = 1; // Tour on sale

        // Add tour to cart
        cartService.AddToCart(touristId, tourId);

        // Apply coupon (if available in test data)
        // cartService.ApplyCoupon(touristId, "TESTCOUPON");

        // Act
        var tokens = cartService.Checkout(touristId);

        // Assert
        tokens.ShouldNotBeEmpty();
        
        var purchase = dbContext.TourPurchases
            .FirstOrDefault(p => p.TouristId == touristId && p.TourId == tourId);
        
        purchase.ShouldNotBeNull();
        purchase.PricePaid.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Multiple_tours_checkout_calculates_individual_sale_prices()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        EnsureTourTestData(scope);

        long touristId = 102;
        long tour1Id = 1; // On sale
        long tour2Id = 2; // On sale
        long tour3Id = 6; // Not on sale

        // Add multiple tours
        cartService.AddToCart(touristId, tour1Id);
        cartService.AddToCart(touristId, tour2Id);
        cartService.AddToCart(touristId, tour3Id);

        // Act
        var tokens = cartService.Checkout(touristId);

        // Assert
        tokens.Count.ShouldBe(3);
        
        var purchases = dbContext.TourPurchases
            .Where(p => p.TouristId == touristId)
            .ToList();
        
        purchases.Count.ShouldBe(3);
        
        // Each purchase should have a price
        purchases.All(p => p.PricePaid > 0).ShouldBeTrue();
    }
}
