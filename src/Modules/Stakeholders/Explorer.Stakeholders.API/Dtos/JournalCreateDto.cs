namespace Explorer.Stakeholders.API.Dtos;

public class JournalCreateDto
{
    public string Title { get; private set; }
    public string Location { get; private set; }
    public string? Content { get; private set; }
}