using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.User;

[Authorize(Policy = "userPolicy")]
[Route("api/notifications")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public ActionResult<PagedResult<NotificationDto>> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.UserId();
        var result = _notificationService.GetByUser(userId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("unread")]
    public ActionResult<List<NotificationDto>> GetUnreadNotifications()
    {
        var userId = User.UserId();
        
        var notifications = _notificationService.GetUnreadByUser(userId);
        return Ok(notifications);
    }

    [HttpPut("{id:long}/mark-read")]
    public ActionResult<NotificationDto> MarkAsRead(long id)
    {
        try
        {
            var notification = _notificationService.MarkAsRead(id);
            return Ok(notification);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:long}/mark-unread")]
    public ActionResult<NotificationDto> MarkAsUnread(long id)
    {
        try
        {
            var notification = _notificationService.MarkAsUnread(id);
            return Ok(notification);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        try
        {
            _notificationService.Delete(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
