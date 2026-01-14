namespace Explorer.Encounters.API.Dtos;

public class EncounterDto
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int Xp { get; set; }
    public string Status { get; set; } = "";
    public string Type { get; set; } = "";
    public int? RequiredPeopleCount { get; set; }
    public double? Range { get; set; }
}