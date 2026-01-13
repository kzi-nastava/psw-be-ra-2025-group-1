namespace Explorer.Encounters.API.Dtos;

public class EncounterCreateDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int Xp { get; set; }
    public string Type { get; set; } = "";
}