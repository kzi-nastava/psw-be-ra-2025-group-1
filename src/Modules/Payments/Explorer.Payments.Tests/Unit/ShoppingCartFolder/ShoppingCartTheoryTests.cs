using Explorer.Tours.Core.Domain.Shopping;
using Explorer.Tours.Core.Domain.Shopping;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit.ShoppingCartFolder
{
    public class ShoppingCartTheoryTests
    {
        // InlineData:

        [Theory]
        [InlineData(1, 10, 50, 1, 1, 50)]   // dodavanje jednom
        [InlineData(1, 10, 50, 2, 2, 100)]  // dodavanje iste ture 2 puta 
        [InlineData(2, 5, 0, 3, 3, 0)]      // besplatna tura
        [InlineData(3, 7, 20, 5, 5, 100)]   // 5 puta -> 5 * 20 = 100
        public void AddItem_behavior(
            long touristId,
            long tourId,
            decimal price,
            int repeatCount,
            int expectedQuantity,
            decimal expectedTotal)
        {
            // Arrange
            var cart = new ShoppingCart(touristId);

            // Act
            for (int i = 0; i < repeatCount; i++)
            {
                cart.AddItem(tourId, "Test Tour", price);
            }

            // Assert
            cart.Items.Count.ShouldBe(1); // samo jedna stavka za isti tourId

            var item = cart.Items.First();

            item.Quantity.ShouldBe(expectedQuantity);
            cart.TotalPrice.ShouldBe(expectedTotal);
        }

        // Test brisanja kao Theory
        [Theory]
        [InlineData(1, 10, 50, true)]
        [InlineData(1, 10, 50, false)]
        public void RemoveItem_behavior(long touristId, long tourId, decimal price, bool removeExisting)
        {
            // Arrange
            var cart = new ShoppingCart(touristId);
            cart.AddItem(tourId, "Tour", price);

            if (!removeExisting)
                tourId = 999; // pokušaj brisanja nepostojeće ture

            // Act
            cart.RemoveItem(tourId);

            // Assert
            if (removeExisting)
            {
                cart.Items.ShouldBeEmpty();
                cart.TotalPrice.ShouldBe(0);
            }
            else
            {
                cart.Items.Count.ShouldBe(1);
                cart.TotalPrice.ShouldBe(price);
            }
        }
    }
}
