using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface INotificationRepository
{
    Notification Create(Notification notification);
    Notification? Get(long id);
    PagedResult<Notification> GetByUserId(long userId, int page, int pageSize);
    List<Notification> GetUnreadByUserId(long userId);
    Notification Update(Notification notification);
    void Delete(long id);
}
