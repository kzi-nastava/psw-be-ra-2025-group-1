using Explorer.Payments.API.Dtos.Sales;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class SaleCommandTests : BasePaymentsIntegrationTest
{
    public SaleCommandTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaleService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var newSale = new CreateSaleDto
        {
            Name = "Summer Sale " + Guid.NewGuid().ToString().Substring(0, 8),
            DiscountPercentage = 20,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            TourIds = new List<long> { 1, 2 }
        };

        // Act
        var result = service.Create(newSale, 1);

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newSale.Name);
        result.DiscountPercentage.ShouldBe(newSale.DiscountPercentage);
        result.AuthorId.ShouldBe(1);

        // Assert - Database
        var storedEntity = dbContext.Sales.FirstOrDefault(s => s.Name == newSale.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_empty_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaleService>();

        var invalidSale = new CreateSaleDto
        {
            Name = "",
            DiscountPercentage = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            TourIds = new List<long> { 1 }
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.Create(invalidSale, 1));
    }

    [Fact]
    public void Create_fails_invalid_discount_percentage()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaleService>();

        var invalidSale = new CreateSaleDto
        {
            Name = "Invalid Sale",
            DiscountPercentage = 150,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            TourIds = new List<long> { 1 }
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.Create(invalidSale, 1));
    }

    [Fact]
    public void Create_fails_duration_exceeds_two_weeks()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaleService>();

        var invalidSale = new CreateSaleDto
        {
            Name = "Long Sale",
            DiscountPercentage = 20,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(15),
            TourIds = new List<long> { 1 }
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.Create(invalidSale, 1));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaleService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var originalSale = new CreateSaleDto
        {
            Name = "Original Sale " + Guid.NewGuid().ToString().Substring(0, 8),
            DiscountPercentage = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            TourIds = new List<long> { 1 }
        };
        var created = service.Create(originalSale, 1);

        var updatedSale = new UpdateSaleDto
        {
            Name = "Updated Sale " + Guid.NewGuid().ToString().Substring(0, 8),
            DiscountPercentage = 25,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            TourIds = new List<long> { 1, 2 }
        };

        // Act
        var result = service.Update(created.Id, updatedSale, 1);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(updatedSale.Name);

        var storedEntity = dbContext.Sales.FirstOrDefault(s => s.Id == created.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedSale.Name);
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaleService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var saleToDelete = new CreateSaleDto
        {
            Name = "Sale To Delete " + Guid.NewGuid().ToString().Substring(0, 8),
            DiscountPercentage = 10,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            TourIds = new List<long> { 1 }
        };
        var created = service.Create(saleToDelete, 1);

        // Act
        service.Delete(created.Id, 1);

        // Assert
        var storedEntity = dbContext.Sales.FirstOrDefault(s => s.Id == created.Id);
        storedEntity.ShouldBeNull();
    }
}