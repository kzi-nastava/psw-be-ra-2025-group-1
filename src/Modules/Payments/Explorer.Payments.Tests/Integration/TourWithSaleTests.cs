using Explorer.Payments.API.Public.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourWithSaleTests : BasePaymentsIntegrationTest
{
    public TourWithSaleTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_published_tours_with_sale_discount()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourBrowsingService>();

        // Act
        var result = service.GetPublished(1, 10);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();

        // Check if any tour has sale applied
        var toursOnSale = result.Results.Where(t => t.IsOnSale).ToList();
        if (toursOnSale.Any())
        {
            foreach (var tour in toursOnSale)
            {
                tour.OriginalPrice.ShouldNotBeNull();
                tour.DiscountedPrice.ShouldNotBeNull();
                tour.SaleDiscountPercentage.ShouldNotBeNull();
                tour.DiscountedPrice.Value.ShouldBeLessThan(tour.OriginalPrice.Value);
            }
        }
    }

    [Fact]
    public void Gets_only_tours_on_sale()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourBrowsingService>();

        // Act
        var result = service.GetToursOnSale(0, 10, null);

        // Assert
        result.ShouldNotBeNull();
        result.Results.All(t => t.IsOnSale).ShouldBeTrue();
        result.Results.All(t => t.OriginalPrice.HasValue).ShouldBeTrue();
        result.Results.All(t => t.DiscountedPrice.HasValue).ShouldBeTrue();
    }

    [Fact]
    public void Tours_on_sale_sorted_by_discount_descending()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourBrowsingService>();

        // Act
        var result = service.GetToursOnSale(0, 10, "discount-desc");

        // Assert
        result.ShouldNotBeNull();
        if (result.Results.Count > 1)
        {
            for (int i = 0; i < result.Results.Count - 1; i++)
            {
                var current = result.Results[i].SaleDiscountPercentage ?? 0;
                var next = result.Results[i + 1].SaleDiscountPercentage ?? 0;
                current.ShouldBeGreaterThanOrEqualTo(next);
            }
        }
    }

    [Fact]
    public void Tour_by_id_includes_sale_info()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourBrowsingService>();

        // Get all tours to find one on sale
        var allTours = service.GetPublished(0, 100);
        var tourOnSale = allTours.Results.FirstOrDefault(t => t.IsOnSale);

        if (tourOnSale == null)
        {
            // Skip test if no tours on sale
            return;
        }

        // Act
        var result = service.GetPublishedById(tourOnSale.Id);

        // Assert
        result.ShouldNotBeNull();
        result.IsOnSale.ShouldBeTrue();
        result.OriginalPrice.ShouldNotBeNull();
        result.DiscountedPrice.ShouldNotBeNull();
        result.SaleDiscountPercentage.ShouldNotBeNull();
        result.SaleId.ShouldNotBeNull();
        result.SaleName.ShouldNotBeNullOrWhiteSpace();
    }
}
