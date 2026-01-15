using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.Shopping;

public class OrderItem : ValueObject
{
    public long Id { get; private set; }

    public long TourId { get; private set; }
    public string TourName { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => Price * Quantity;

    public OrderItem(long tourId, string tourName, decimal price, int quantity)
    {
        TourId = tourId;
        TourName = tourName;
        Price = price;
        Quantity = quantity;
    }

    public void IncreaseQuantity()
    {
        Quantity++;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TourId;
        yield return Price;
    }
}
