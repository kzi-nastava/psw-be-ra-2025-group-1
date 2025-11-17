using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IMeetUpService
{
    PagedResult<MeetUpDto> GetPaged(int page, int pageSize);
    PagedResult<MeetUpDto> GetPagedByUser(long userId, int page, int pageSize);
    MeetUpDto Create(MeetUpDto meetup);
    MeetUpDto Update(MeetUpDto meetup);
    void Delete(long id);
}