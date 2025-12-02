using Explorer.API.Controllers.User;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Notifications;

[Collection("Sequential")]
public class NotificationCommandTests : BaseStakeholdersIntegrationTest
{
    public NotificationCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_user_notifications()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Act
        var result = ((ObjectResult)controller.GetMyNotifications(1, 10).Result)?.Value as PagedResult<NotificationDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.ShouldAllBe(n => n.UserId == -11);
    }

    [Fact]
    public void Get_unread_notifications()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Act
        var result = ((ObjectResult)controller.GetUnreadNotifications().Result)?.Value as List<NotificationDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldAllBe(n => n.UserId == -11 && !n.IsRead);
    }

    [Fact]
    public void Mark_notification_as_read()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, "-11");

        // Act
        var result = ((ObjectResult)controller.MarkAsRead(-1).Result)?.Value as NotificationDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.IsRead.ShouldBeTrue();

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedNotification = dbContext.Notifications.Find(-1L);
        storedNotification.ShouldNotBeNull();
        storedNotification.IsRead.ShouldBeTrue();
    }

    [Fact]
    public void Mark_notification_as_unread()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, "-11");

        // Act
        var result = ((ObjectResult)controller.MarkAsUnread(-3).Result)?.Value as NotificationDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-3);
        result.IsRead.ShouldBeFalse();

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedNotification = dbContext.Notifications.Find(-3L);
        storedNotification.ShouldNotBeNull();
        storedNotification.IsRead.ShouldBeFalse();
    }

    [Fact]
    public void Delete_notification()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, "-22");

        // Act
        var result = (NoContentResult)controller.Delete(-4);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedNotification = dbContext.Notifications.Find(-4L);
        storedNotification.ShouldBeNull();
    }

    [Fact]
    public void Mark_as_read_fails_for_nonexistent_notification()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Act
        var result = controller.MarkAsRead(-999);

        // Assert
        result.Result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public void Delete_fails_for_nonexistent_notification()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

        // Act & Assert - Service layer should throw KeyNotFoundException
        Should.Throw<KeyNotFoundException>(() => service.Delete(-999));
    }

    private static NotificationController CreateController(IServiceScope scope, string personId)
    {
        return new NotificationController(
            scope.ServiceProvider.GetRequiredService<INotificationService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}
