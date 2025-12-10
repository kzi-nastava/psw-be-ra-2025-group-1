using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Administration
{
    public interface ITransportTimeService
    {
        IEnumerable<TransportTimeDto> GetAll();
        PagedResult<TransportTimeDto> GetPaged(int page, int pageSize);
        TransportTimeDto Get(long id);
        TransportTimeDto Create(TransportTimeDto transportTimeDto);
        TransportTimeDto Update(TransportTimeDto transportTimeDto);
        IEnumerable<TransportTimeDto> GetByTourId(long tourId);
        IEnumerable<TransportTimeDto> GetByTransportType(TransportTypeDto transport);
        void Delete(long id);
    }
}
