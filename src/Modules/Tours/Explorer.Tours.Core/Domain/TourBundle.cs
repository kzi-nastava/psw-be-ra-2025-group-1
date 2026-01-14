using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.Core.Domain;

public enum TourBundleStatus
{
    Draft,
    Published,
    Archived
}

public class TourBundle : AggregateRoot
{
    public long CreatorId { get; private set; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public List<Tour> Tours { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public TourBundleStatus Status { get; private set; }
    public DateTime PublishedAt { get; private set; }
    public DateTime ArchivedAt { get; private set; }

    public TourBundle()
    {
        Name = "";
        Price = 0;
        Tours = new List<Tour>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = TourBundleStatus.Draft;
    }

    public TourBundle(long creatorId, string name, List<Tour> tours, double price = 0)
    {
        CreatorId = creatorId;
        Name = name;
        Tours = tours ?? new List<Tour>();
        Price = price;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = TourBundleStatus.Draft;
    }

    public void AddTour(Tour tour)
    {
        if (Tours.Any(t => t.Id == tour.Id))
            throw new InvalidOperationException("Tour is already in the bundle");

        Tours.Add(tour);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTour(long tourId)
    {
        if (Status == TourBundleStatus.Published)
            throw new InvalidOperationException("Cannot remove tours from a published bundle");

        var tour = Tours.FirstOrDefault(t => t.Id == tourId)
            ?? throw new InvalidOperationException("Tour not found in bundle");

        Tours.Remove(tour);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, double price)
    {
        if (Status == TourBundleStatus.Published)
            throw new InvalidOperationException("Cannot update a published bundle");

        Name = name;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public double GetTotalToursPrice()
    {
        return Tours.Sum(t => t.Price);
    }

    public bool CanPublish()
    {
        return Tours.Count(t => t.Status == TourStatus.Published) >= 2;
    }

    public void Publish()
    {
        if (Status != TourBundleStatus.Draft)
            throw new InvalidOperationException("Only draft bundles can be published");

        if (!CanPublish())
            throw new InvalidOperationException("Bundle must contain at least 2 published tours to be published");

        Status = TourBundleStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status != TourBundleStatus.Published)
            throw new InvalidOperationException("Only published bundles can be archived");

        Status = TourBundleStatus.Archived;
        ArchivedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status != TourBundleStatus.Archived)
            throw new InvalidOperationException("Only archived bundles can be activated");

        Status = TourBundleStatus.Published;
    }
}
