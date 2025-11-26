using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Rating : Entity
{
    public long UserId { get; private set; }
    public int Score { get; private set; }          // 1–5
    public string? Comment { get; private set; }    // ≤ 500 (opciono)

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core trazi prazan ctor za materializaciju iz baze
    protected Rating() { }

    public Rating(long userId, int score, string? comment)
    {
        UserId = userId;
        Score = score;
        Comment = Normalize(comment);
        CreatedAt = DateTime.UtcNow;
        Validate();
    }

    public void Update(int score, string? comment)
    {
        Score = score;
        Comment = Normalize(comment);
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    private static string? Normalize(string? comment) =>
        string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid UserId");
        if (Score < 1 || Score > 5) throw new ArgumentException("Score must be between 1 and 5.");
        if (Comment != null && Comment.Length > 500) throw new ArgumentException("Comment too long (max 500).");
    }
}
