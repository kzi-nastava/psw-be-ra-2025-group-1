using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class ProblemMessage : Entity
{
    public long ProblemId { get; init; }
    public long AuthorId { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Content { get; init; }

    public ProblemMessage(long problemId, long authorId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty.");
        
        ProblemId = problemId;
        AuthorId = authorId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }

    private ProblemMessage() 
    { 
        Content = string.Empty;
    }
}
