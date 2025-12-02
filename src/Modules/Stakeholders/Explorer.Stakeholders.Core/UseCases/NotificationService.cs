using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public NotificationDto Create(long userId, string message, NotificationTypeDto type, long? linkId = null)
    {
        var notificationType = _mapper.Map<NotificationType>(type);
        var notification = new Notification(userId, message, notificationType, linkId);
        var result = _notificationRepository.Create(notification);
        return _mapper.Map<NotificationDto>(result);
    }

    public PagedResult<NotificationDto> GetByUser(long userId, int page, int pageSize)
    {
        var result = _notificationRepository.GetByUserId(userId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<NotificationDto>).ToList();
        return new PagedResult<NotificationDto>(items, result.TotalCount);
    }

    public List<NotificationDto> GetUnreadByUser(long userId)
    {
        var notifications = _notificationRepository.GetUnreadByUserId(userId);
        return notifications.Select(_mapper.Map<NotificationDto>).ToList();
    }

    public NotificationDto MarkAsRead(long notificationId)
    {
        var notification = _notificationRepository.Get(notificationId);
        if (notification == null)
            throw new KeyNotFoundException($"Notification with id {notificationId} not found.");

        notification.MarkAsRead();
        var result = _notificationRepository.Update(notification);
        return _mapper.Map<NotificationDto>(result);
    }

    public NotificationDto MarkAsUnread(long notificationId)
    {
        var notification = _notificationRepository.Get(notificationId);
        if (notification == null)
            throw new KeyNotFoundException($"Notification with id {notificationId} not found.");

        notification.MarkAsUnread();
        var result = _notificationRepository.Update(notification);
        return _mapper.Map<NotificationDto>(result);
    }

    public void Delete(long notificationId)
    {
        var notification = _notificationRepository.Get(notificationId);
        if (notification == null)
            throw new KeyNotFoundException($"Notification with id {notificationId} not found.");
        
        _notificationRepository.Delete(notificationId);
    }
}
