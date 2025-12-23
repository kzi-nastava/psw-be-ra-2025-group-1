namespace Explorer.Encounters.API.Dtos;

public class EncounterCreateDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Location { get; set; } = "";
    public int Xp { get; set; }
    public string Type { get; set; } = "";
}