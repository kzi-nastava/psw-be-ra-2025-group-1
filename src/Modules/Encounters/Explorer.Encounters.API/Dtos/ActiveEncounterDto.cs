namespace Explorer.Encounters.API.Dtos;

public class ActiveEncounterDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long EncounterId { get; set; }
    public int EncounterXp { get; set; }
    public DateTime ActivationTime { get; set; }
    public DateTime LastLocationUpdate { get; set; }
    public double LastLatitude { get; set; }
    public double LastLongitude { get; set; }
    public bool IsWithinRange { get; set; }
    
    // Optional encounter details
    public string? EncounterTitle { get; set; }
    public string? EncounterDescription { get; set; }
    public string? EncounterType { get; set; }
    public int? RequiredPeopleCount { get; set; }
    public int? CurrentPeopleInRange { get; set; }
    public List<RequirementDto>? Requirements { get; set; } = new List<RequirementDto>();
    public string? ImagePath { get; set; }
}
