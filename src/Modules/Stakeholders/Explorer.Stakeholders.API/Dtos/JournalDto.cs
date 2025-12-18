namespace Explorer.Stakeholders.API.Dtos;

public enum Status 
{
    Draft,
    Finished
}

public class JournalDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status{ get; set; }
    public string Location { get; set; }
    public long UserId { get; set; }
    public string? Content { get; set; }
}