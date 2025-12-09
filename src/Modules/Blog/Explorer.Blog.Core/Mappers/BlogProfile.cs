using AutoMapper;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Domain.Blog, BlogDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap();
        CreateMap<Domain.Comment, CommentDto>().ReverseMap();
        CreateMap<Domain.Vote, VoteDto>().ReverseMap();
    }
}