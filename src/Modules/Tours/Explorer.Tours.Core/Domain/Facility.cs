using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum FacilityCategory
{
    WC,
    Restaurant,
    Parking,
    Other
}

public class Facility : Entity
{
    public string Name { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public FacilityCategory Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public Facility(string name, double latitude, double longitude, FacilityCategory category)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (latitude < -90 || latitude > 90) throw new ArgumentException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180) throw new ArgumentException("Longitude must be between -180 and 180.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    // Private constructor for EF Core
    private Facility() { }

    public void Update(string name, double latitude, double longitude, FacilityCategory category)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (latitude < -90 || latitude > 90) throw new ArgumentException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180) throw new ArgumentException("Longitude must be between -180 and 180.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}
