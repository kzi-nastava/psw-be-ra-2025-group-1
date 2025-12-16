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
    public bool Publish(long id);
    public bool Archive(long id);
    TourDto Create(CreateTourDto tour);
    TourDto Update(long id, TourDto tour, long authorId);
    void Delete(long id, long authorId);
    KeypointDto AddKeypoint(long tourId, KeypointDto keypointDto, long authorId);
    KeypointDto UpdateKeypoint(long tourId, KeypointDto keypointDto, long authorId);
    void DeleteKeypoint(long tourId, long keypointId, long authorId);
}
