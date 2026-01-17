using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Mappers;

public class EncounterProfile : Profile
{
    public EncounterProfile()
    {
        CreateMap<Encounter, EncounterDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));

        CreateMap<Requirement, RequirementDto>().ReverseMap();

        CreateMap<TouristStats, TouristStatsDto>().ReverseMap();
        CreateMap<Encounter, HiddenEncounterDto>().ReverseMap();
    }
}