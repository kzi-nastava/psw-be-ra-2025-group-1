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
        CreateMap<CreateTourDto, Tour>().ForMember(dest => dest.Keypoints, opt => opt.Ignore()).ForMember(dest => dest.Equipment, opt => opt.Ignore());
        CreateMap<KeypointDto, Keypoint>().ReverseMap();
        CreateMap<TransportTimeDto, TransportTime>().ReverseMap();
        CreateMap<TourRatingDto, TourRating>().ReverseMap();
        CreateMap<TourRatingReactionDto, TourRatingReaction>().ReverseMap();
        CreateMap<Explorer.Tours.API.Dtos.EncounterCreateDto, Explorer.Encounters.API.Dtos.EncounterCreateDto>().ReverseMap();
        CreateMap<Explorer.Tours.API.Dtos.EncounterDto, Explorer.Encounters.API.Dtos.EncounterDto>().ReverseMap();

        CreateMap<MonumentDto, Monument>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<MonumentStatus>(src.Status)));

        CreateMap<Monument, MonumentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<KeypointProgressDto, KeypointProgress>().ReverseMap();

        // TourExecution mappings with explicit enum conversion
        CreateMap<TourExecution, TourExecutionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourExecutionStatusDto)src.Status));
        
        CreateMap<TourExecutionDto, TourExecution>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourExecutionStatus)src.Status));
        
        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(d => d.DistanceKm, opt => opt.Ignore());

    }
}