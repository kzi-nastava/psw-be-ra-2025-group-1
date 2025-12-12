namespace Explorer.Stakeholders.API.Dtos;

public class AddProblemMessageDto
{
    public long ProblemId { get; set; }
    public long AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
}
