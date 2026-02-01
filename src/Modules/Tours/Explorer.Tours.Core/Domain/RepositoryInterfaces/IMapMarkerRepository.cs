using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IMapMarkerRepository
    {
        PagedResult<MapMarker> GetPaged(int page, int pageSize);
        List<MapMarker> GetAll();
        MapMarker Get(long mapMarkerId);
        MapMarker GetByImageUrl(string imageUrl);
        MapMarker Create(MapMarker mapMarker);
        MapMarker Update(MapMarker mapMarker);
        void Delete(long mapMarkerId);
    }
}
