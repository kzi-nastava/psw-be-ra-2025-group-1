using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.Shopping;

public class ShoppingCart : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public long TouristId { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalPrice => _items.Sum(i => i.TotalPrice);

    protected ShoppingCart() { }

    public ShoppingCart(long touristId)
    {
        TouristId = touristId;
    }

    public void AddItem(long tourId, string name, decimal price)
    {
        var existing = _items.FirstOrDefault(i => i.TourId == tourId);
        if (existing != null)
        {
            existing.IncreaseQuantity();
            return;
        }

        _items.Add(new OrderItem(tourId, name, price, 1));
    }

    public void RemoveItem(long tourId)
    {
        var item = _items.SingleOrDefault(i => i.TourId == tourId);
        if (item == null) return;

        _items.Remove(item);
    }
    
    public void Clear()
    {
        _items.Clear();
    }
}
