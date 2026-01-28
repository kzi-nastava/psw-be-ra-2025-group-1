namespace Explorer.Tours.API.Dtos;

public enum FacilityCategory
{
    WC,
    Store,
    Parking,
    Other
}

public enum EstimatedPrice
{
    Cheap,
    Average,
    Pricy
}

public class FacilityDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public FacilityCategory Category { get; set; }
    public long? CreatorId { get; set; }
    public bool IsLocalPlace { get; set; }
    public EstimatedPrice EstimatedPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
