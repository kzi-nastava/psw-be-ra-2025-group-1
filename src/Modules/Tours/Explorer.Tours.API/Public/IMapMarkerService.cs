using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface IMapMarkerService
    {
        PagedResult<MapMarkerDto> GetPaged(int page, int pageSize);
        List<MapMarkerDto> GetAll();
        MapMarkerDto Get(long mapMarkerId);
        MapMarkerDto Create(MapMarkerDto mapMarkerDto);
        MapMarkerDto Update(MapMarkerDto mapMarkerDto);
        void Delete(long id);
    }
}
