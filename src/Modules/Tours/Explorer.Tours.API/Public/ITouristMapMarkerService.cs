using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface ITouristMapMarkerService
    {
        PagedResult<TouristMapMarkerDto> GetPagedByTourist(int page, int pageSize, long touristId);
        List<TouristMapMarkerDto> GetAllByTourist(long touristId);
        TouristMapMarkerDto GetActiveByTourist(long touristId);
        //void Delete(long id);

        TouristMapMarkerDto SetMapMarkerAsActive(long touristId, long mapMarkerId);
        TouristMapMarkerDto Collect(long touristId, long mapMarkerId);
        TouristMapMarkerDto CollectFromTour(long touristId, long tourId);
    }
}
