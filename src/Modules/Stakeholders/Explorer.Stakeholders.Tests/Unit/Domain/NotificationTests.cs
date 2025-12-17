using Explorer.Stakeholders.Core.Domain;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit.Domain;

public class NotificationTests
{
    [Fact]
    public void Creates_notification_with_valid_data()
    {
        // Arrange & Act
        var notification = new Notification(-11, "Test notification", NotificationType.ProblemReportMessage, -1);

        // Assert
        notification.UserId.ShouldBe(-11);
        notification.Message.ShouldBe("Test notification");
        notification.Type.ShouldBe(NotificationType.ProblemReportMessage);
        notification.LinkId.ShouldBe(-1);
        notification.Timestamp.ShouldNotBe(default);
        notification.IsRead.ShouldBeFalse();
    }

    [Fact]
    public void Creates_notification_without_link()
    {
        // Arrange & Act
        var notification = new Notification(-11, "General notification", NotificationType.Other);

        // Assert
        notification.LinkId.ShouldBeNull();
        notification.Type.ShouldBe(NotificationType.Other);
    }

    [Fact]
    public void Marks_notification_as_read()
    {
        // Arrange
        var notification = new Notification(-11, "Test notification", NotificationType.ProblemReportMessage, -1);

        // Act
        notification.MarkAsRead();

        // Assert
        notification.IsRead.ShouldBeTrue();
    }

    [Fact]
    public void Marks_notification_as_unread()
    {
        // Arrange
        var notification = new Notification(-11, "Test notification", NotificationType.ProblemReportMessage, -1);
        notification.MarkAsRead();

        // Act
        notification.MarkAsUnread();

        // Assert
        notification.IsRead.ShouldBeFalse();
    }

    [Theory]
    [InlineData(NotificationType.ProblemReportMessage)]
    [InlineData(NotificationType.ProblemReportDeadline)]
    [InlineData(NotificationType.Other)]
    public void Creates_notification_with_different_types(NotificationType type)
    {
        // Arrange & Act
        var notification = new Notification(-11, "Test notification", type, -1);

        // Assert
        notification.Type.ShouldBe(type);
    }

    [Fact]
    public void Toggle_read_status_multiple_times()
    {
        // Arrange
        var notification = new Notification(-11, "Test notification", NotificationType.ProblemReportMessage, -1);

        // Act & Assert
        notification.IsRead.ShouldBeFalse();
        
        notification.MarkAsRead();
        notification.IsRead.ShouldBeTrue();
        
        notification.MarkAsUnread();
        notification.IsRead.ShouldBeFalse();
        
        notification.MarkAsRead();
        notification.IsRead.ShouldBeTrue();
    }
}
