using Explorer.Payments.API.Dtos.Sales;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class SaleQueryTests : BasePaymentsIntegrationTest
{
    public SaleQueryTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_by_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaleService>();

        var newSale = new CreateSaleDto
        {
            Name = "Author Sale " + Guid.NewGuid().ToString().Substring(0, 8),
            DiscountPercentage = 20,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            TourIds = new List<long> { 1 }
        };
        service.Create(newSale, 1);

        // Act
        var result = service.GetByAuthor(1);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.All(s => s.AuthorId == 1).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_active_sales()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorService = scope.ServiceProvider.GetRequiredService<ISaleService>();
        var touristService = scope.ServiceProvider.GetRequiredService<ISalePublicService>();

        var activeSale = new CreateSaleDto
        {
            Name = "Active Sale " + Guid.NewGuid().ToString().Substring(0, 8),
            DiscountPercentage = 30,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(5),
            TourIds = new List<long> { 1 }
        };
        var created = authorService.Create(activeSale, 1);

        // Act
        var result = touristService.GetActiveSales();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain(s => s.Id == created.Id);
    }
}