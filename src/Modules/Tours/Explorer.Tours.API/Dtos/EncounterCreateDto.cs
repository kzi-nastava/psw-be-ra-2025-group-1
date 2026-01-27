namespace Explorer.Tours.API.Dtos;

public class EncounterCreateDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int Xp { get; set; }
    public string Type { get; set; } = "";
    public int? RequiredPeopleCount { get; set; }
    public List<string> Requirements { get; set; } = new List<string>();
    public double? Range { get; set; }
    public string? ImagePath { get; set; }
    public List<string>? Hints { get; set; }
    public long? KeypointId { get; set; }
}