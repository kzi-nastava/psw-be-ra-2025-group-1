using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain
{
    public interface ITransportTimeRepository
    {
        PagedResult<TransportTime> GetPaged(int page, int pageSize);
        IEnumerable<TransportTime> GetByTourId(long tourId);
        IEnumerable<TransportTime> GetByTransportType(TransportType transport);
        TransportTime Get(long id);
        TransportTime Create(TransportTime entity);
        TransportTime Update(TransportTime entity);
        void Delete(long id);
    }
}
