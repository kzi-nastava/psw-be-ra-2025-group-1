using System;

namespace Explorer.Stakeholders.API.Dtos;

public enum NotificationTypeDto
{
    ProblemReportMessage,
    ProblemReportDeadline,
    Other
}

public class NotificationDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public long? LinkId { get; set; }
    public NotificationTypeDto Type { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
}
