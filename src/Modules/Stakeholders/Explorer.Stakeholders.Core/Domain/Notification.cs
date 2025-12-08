using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum NotificationType
{
    ProblemReportMessage,
    ProblemReportDeadline,
    ProblemDeadlineExpired,
    ProblemClosedByAdmin,
    AuthorPenalized,
    Other
}

public class Notification : Entity
{
    public long UserId { get; init; }
    public string Message { get; init; }
    public long? LinkId { get; init; } // id povezanog entiteta
    public NotificationType Type { get; init; }
    public DateTime Timestamp { get; init; }
    public bool IsRead { get; private set; }

    public Notification(long userId, string message, NotificationType type, long? linkId = null)
    {
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message cannot be empty.");
        
        UserId = userId;
        Message = message;
        Type = type;
        LinkId = linkId;
        Timestamp = DateTime.UtcNow;
        IsRead = false;
    }

    private Notification() 
    {
        Message = string.Empty;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
    }
}
