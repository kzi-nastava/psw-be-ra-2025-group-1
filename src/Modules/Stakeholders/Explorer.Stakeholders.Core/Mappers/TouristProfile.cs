using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class TouristProfile : Profile
    {
        public TouristProfile()
        {
            CreateMap<TourPreferenceDto, TourPreference>().ReverseMap();
        }
    }
}
