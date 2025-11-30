using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Administration;

public interface ITourService
{
    PagedResult<TourDto> GetPaged(int page, int pageSize);
    TourDto GetById(long id);
    List<TourDto> GetAll();

    PagedResult<TourDto> GetByCreator(long creatorId, int page, int pageSize);
    TourDto Create(CreateTourDto tour);
    TourDto Update(long id, TourDto tour);
    void Delete(long id);

    KeypointDto AddKeypoint(long tourId, KeypointDto keypointDto);
    KeypointDto UpdateKeypoint(long tourId, KeypointDto keypointDto);
    void DeleteKeypoint(long tourId, long keypointId);
}
