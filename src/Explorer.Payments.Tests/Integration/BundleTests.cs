using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Payments.Core.Domain.Bundles;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class BundleTests : BasePaymentsIntegrationTest
{
    public BundleTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBundleService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var newBundle = new BundleCreationDto
        {
            Name = "New Bundle",
            Price = 50.0m,
            TourIds = new List<long> { -1, -2 }
        };

        // Act
        var result = service.Create(-11, newBundle);

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newBundle.Name);
        result.Price.ShouldBe(newBundle.Price);
        result.AuthorId.ShouldBe(-11);
        result.Status.ShouldBe("Draft");

        // Assert - Database
        var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(newBundle.Name);
    }

    [Fact]
    public void Publishes_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBundleService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Bundle -1 has tours 1 and 2 (both published in f-tours.sql)

        // Act
        var result = service.Publish(-11, -1);

        // Assert
        result.Status.ShouldBe("Published");
        
        var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(BundleStatus.Published);
    }

    [Fact]
    public void Deletes_fails_if_published()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

        // Bundle -2 is seeded as Published

        // Act & Assert
        Should.Throw<Exception>(() => service.Delete(-11, -2));
    }

    [Fact]
    public void Archives_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBundleService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Bundle -2 is seeded as Published

        // Act
        var result = service.Archive(-11, -3);

        // Assert
        result.Status.ShouldBe("Archived");

        var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Id == -3);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(BundleStatus.Archived);
    }
}
