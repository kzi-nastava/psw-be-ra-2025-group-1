using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.Sales;

public class Sale : Entity
{
    public string Name { get; private set; }
    public int DiscountPercentage { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public long AuthorId { get; private set; }
    public List<long> TourIds { get; private set; }

    public Sale(string name, int discountPercentage, DateTime startDate, DateTime endDate, long authorId)
    {
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        AuthorId = authorId;
        TourIds = new List<long>();

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Sale name cannot be empty.");

        if (DiscountPercentage <= 0 || DiscountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 1 and 100.");

        if (EndDate <= StartDate)
            throw new ArgumentException("End date must be after start date.");

        var maxDuration = TimeSpan.FromDays(14);
        if ((EndDate - StartDate) > maxDuration)
            throw new ArgumentException("Sale duration cannot exceed 2 weeks.");
    }

    public void Update(string name, int discountPercentage, DateTime startDate, DateTime endDate)
    {
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;

        Validate();
    }

    public void AddTourId(long tourId, long tourAuthorId)
    {
        // Jednostavno dodaj - bez provere
        if (!TourIds.Contains(tourId))
            TourIds.Add(tourId);
    }

    public void RemoveTourId(long tourId)
    {
        TourIds.Remove(tourId);
    }

    public void ClearTours()
    {
        TourIds.Clear();
    }

    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        return now >= StartDate && now <= EndDate;
    }
}