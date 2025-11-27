namespace Explorer.Stakeholders.API.Dtos;

public class JournalCreateDto
{
    public string Title { get; set; }
    public string Location { get; set; }
    public string? Content { get; set; }
}