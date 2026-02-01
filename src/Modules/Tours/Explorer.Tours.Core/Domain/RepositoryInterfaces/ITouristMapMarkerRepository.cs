using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITouristMapMarkerRepository
    {
        PagedResult<TouristMapMarker> GetPagedByTourist(int page, int pageSize, long touristId);
        List<TouristMapMarker> GetAllByTourist(long touristId);
        TouristMapMarker GetActiveByTourist(long touristId);
        TouristMapMarker Create(TouristMapMarker touristMapMarker);
        TouristMapMarker Update(TouristMapMarker updatedTouristMapMarker);
        void Delete(long id);

    }
}
