using System;

namespace Explorer.Stakeholders.API.Dtos;

public class ProblemMessageDto
{
    public long Id { get; set; }
    public long ProblemId { get; set; }
    public long AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; } = string.Empty;
}
