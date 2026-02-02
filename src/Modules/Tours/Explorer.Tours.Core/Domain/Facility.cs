using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum FacilityCategory
{
    WC,
    Store,
    Parking,
    Restaurant,
    Other
}

public enum EstimatedPrice
{
    Cheap,
    Average,
    Pricy
}

public class Facility : Entity
{
    public string Name { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public FacilityCategory Category { get; private set; }
    public long? CreatorId { get; private set; }    
    public bool IsLocalPlace { get; private set; }    
    public EstimatedPrice EstimatedPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public Facility(string name, double latitude, double longitude, FacilityCategory category, EstimatedPrice estimatedPrice, long? creatorId = null, bool isLocalPlace = false)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (latitude < -90 || latitude > 90) throw new ArgumentException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180) throw new ArgumentException("Longitude must be between -180 and 180.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
        CreatorId = creatorId;
        IsLocalPlace = isLocalPlace;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
        EstimatedPrice = estimatedPrice;
    }

    // Private constructor for EF Core
    private Facility()
    {
        Name = string.Empty;
        Latitude = 0;
        Longitude = 0;
        Category = FacilityCategory.Other;
        CreatorId = null;
        IsLocalPlace = false;
        EstimatedPrice = EstimatedPrice.Average;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
        IsDeleted = false;
    }

    public void Update(string name, double latitude, double longitude, FacilityCategory category, EstimatedPrice estimatedPrice)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (latitude < -90 || latitude > 90) throw new ArgumentException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180) throw new ArgumentException("Longitude must be between -180 and 180.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
        EstimatedPrice = estimatedPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}
