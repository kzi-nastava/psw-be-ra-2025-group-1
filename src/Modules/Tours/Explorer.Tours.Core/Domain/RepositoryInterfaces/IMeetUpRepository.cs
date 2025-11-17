using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IMeetUpRepository
{
    PagedResult<MeetUp> GetPaged(int page, int pageSize);
    PagedResult<MeetUp> GetPagedByUser(long userId, int page, int pageSize);
    MeetUp Create(MeetUp map);
    MeetUp Update(MeetUp map);
    void Delete(long id);
}