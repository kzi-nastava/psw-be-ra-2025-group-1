using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.TourPurchaseTokens;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap();
        CreateMap<FacilityDto, Facility>().ReverseMap();
        CreateMap<MeetUpDto, MeetUp>().ReverseMap();
        CreateMap<PersonEquipmentDto, PersonEquipment>().ReverseMap(); //dodala sam
        CreateMap<TourPurchaseTokenDto, TourPurchaseToken>().ReverseMap();
        CreateMap<CreateTourDto, Tour>().ForMember(dest => dest.Keypoints, opt => opt.Ignore()).ForMember(dest => dest.Equipment, opt => opt.Ignore());
        CreateMap<KeypointDto, Keypoint>().ReverseMap();
        CreateMap<PersonEquipmentDto, PersonEquipment>().ReverseMap();
        CreateMap<TransportTimeDto, TransportTime>().ReverseMap();
        CreateMap<KeypointProgressDto, KeypointProgress>().ReverseMap();

        // TourExecution mappings with explicit enum conversion
        CreateMap<TourExecution, TourExecutionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourExecutionStatusDto)src.Status));
        
        CreateMap<TourExecutionDto, TourExecution>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourExecutionStatus)src.Status));
    }
}