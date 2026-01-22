using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.Bundles;

public enum BundleStatus
{
    Draft,
    Published,
    Archived
}

public class Bundle : AggregateRoot
{
    public long AuthorId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public BundleStatus Status { get; private set; }
    public List<long> TourIds { get; private set; }

    protected Bundle() {}

    public Bundle(long authorId, string name, decimal price, List<long> tourIds)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (price < 0) throw new ArgumentException("Price cannot be negative");
        if (tourIds == null || tourIds.Count == 0) throw new ArgumentException("Bundle must contain at least one tour");

        AuthorId = authorId;
        Name = name;
        Price = price;
        Status = BundleStatus.Draft;
        TourIds = tourIds;
    }

    public void Update(string name, decimal price, List<long> tourIds)
    {
        if (Status == BundleStatus.Published) throw new InvalidOperationException("Cannot update a published bundle");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (price < 0) throw new ArgumentException("Price cannot be negative");
        if (tourIds == null || tourIds.Count == 0) throw new ArgumentException("Bundle must contain at least one tour");

        Name = name;
        Price = price;
        TourIds = tourIds;
    }

    public void Publish(int publishedTourCount)
    {
        if (Status != BundleStatus.Draft) throw new InvalidOperationException("Only draft bundles can be published");
        if (publishedTourCount < 2) throw new InvalidOperationException("At least two tours must be published to publish the bundle");
        
        Status = BundleStatus.Published;
    }

    public void Archive()
    {
        if (Status != BundleStatus.Published) throw new InvalidOperationException("Only published bundles can be archived");
        Status = BundleStatus.Archived;
    }
}
