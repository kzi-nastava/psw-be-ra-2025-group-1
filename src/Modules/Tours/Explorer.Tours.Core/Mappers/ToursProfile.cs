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
        CreateMap<TransportTimeDto, TransportTime>().ReverseMap();
        
        // TourExecution mappings with explicit enum conversion
        CreateMap<TourExecution, TourExecutionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourExecutionStatusDto)src.Status));
        
        CreateMap<TourExecutionDto, TourExecution>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourExecutionStatus)src.Status));
    }
}