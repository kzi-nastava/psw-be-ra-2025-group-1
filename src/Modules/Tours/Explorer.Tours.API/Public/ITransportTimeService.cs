using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface ITransportTimeService
    {
        IEnumerable<TransportTimeDto> GetAllTransportTimes();
        TransportTimeDto GetById(long id);
        TransportTimeDto Create(TransportTimeDto transportTimeDto);
        TransportTimeDto Update(TransportTimeDto transportTimeDto);
        IEnumerable<TransportTimeDto> GetByTourId(long tourId);
        IEnumerable<TransportTimeDto> GetByTransportType(TransportTypeDto transport);
        void DeleteTransportTime(long id);
    }
}
