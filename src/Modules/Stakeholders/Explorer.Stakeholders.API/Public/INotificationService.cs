using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface INotificationService
{
    NotificationDto Create(long userId, string message, NotificationTypeDto type, long? linkId = null);
    PagedResult<NotificationDto> GetByUser(long userId, int page, int pageSize);
    List<NotificationDto> GetUnreadByUser(long userId);
    NotificationDto MarkAsRead(long notificationId);
    NotificationDto MarkAsUnread(long notificationId);
    void Delete(long notificationId);
}
