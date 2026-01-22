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
    TourDto GetById(long tourId);
    List<TourDto> GetAll();
    PagedResult<TourDto> GetByCreator(long creatorId, int page, int pageSize);
    void ArchiveTour(long tourId);
    public bool Publish(long tourId);
    public bool Archive(long tourId);
    public bool Activate(long tourId);  
    TourDto Create(CreateTourDto tour);
    TourDto Update(long tourId, TourDto tour, long authorId);
    void Delete(long tourId, long authorId);

    KeypointDto AddKeypoint(long tourId, KeypointDto keypointDto, long authorId);
    KeypointDto UpdateKeypoint(long tourId, KeypointDto keypointDto, long authorId);
    void DeleteKeypoint(long tourId, long keypointId, long authorId);

    TourDto AddEquipment(long tourId, long equipmentId, long authorId);
    TourDto RemoveEquipment(long tourId, long equipmentId, long authorId);

    TransportTimeDto AddTransportTime(long tourId, TransportTimeDto transportTimeDto, long authorId);
    TransportTimeDto UpdateTransportTime(long tourId, TransportTimeDto transportTimeDto, long authorId);
    void DeleteTransportTime(long tourId, long transportTimeId, long authorId);

    MapMarkerDto AddMapMarker(long tourId, MapMarkerDto mapMarkerDto, long authorId);
    MapMarkerDto UpdateMapMarker(long tourId, MapMarkerDto mapMarkerDto, long authorId);
    void DeleteMapMarker(long tourId, long mapMarkerId, long authorId);
}
