namespace Explorer.Stakeholders.API.Dtos;

public class JournalCreateDto
{
    public string Title { get; set; }
    public string? Content { get; set; }// +markdown
    public string LocationName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<string> Images { get; set; } = new();
    public List<string> Videos { get; set; } = new();

    public bool PublishAsBlog { get; set; } // opcija pri kreiranju
}