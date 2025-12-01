using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain
{
    public interface ITransportTimeRepository
    {
        PagedResult<TransportTime> GetPaged(int page, int pageSize);
        TransportTime Get(long id);
        TransportTime Create(TransportTime transport);
        TransportTime Update(TransportTime transport);
        void Delete(long id);
    }
}
