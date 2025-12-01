using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap();
        CreateMap<FacilityDto, Facility>().ReverseMap();
        CreateMap<MeetUpDto, MeetUp>().ReverseMap();
        CreateMap<PersonEquipmentDto, PersonEquipment>().ReverseMap();
        
        CreateMap<ProblemMessageDto, ProblemMessage>().ReverseMap();
    }
}