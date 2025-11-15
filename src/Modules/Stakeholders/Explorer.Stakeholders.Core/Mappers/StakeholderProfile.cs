using AutoMapper;
using Explorer.Stakeholders.API.Dtos;      //  DTO-i
using Explorer.Stakeholders.Core.Domain;   //  Rating domen

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<Rating, RatingDto>();     //domain -> dto
        CreateMap<RatingCreateDto, Rating>();
        CreateMap<RatingUpdateDto, Rating>();

    }
}