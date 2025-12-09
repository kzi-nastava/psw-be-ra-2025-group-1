using AutoMapper;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Domain.Blog, BlogDto>().ReverseMap();
        CreateMap<Domain.Comment, CommentDto>().ReverseMap();
        CreateMap<Domain.Vote, VoteDto>().ReverseMap();
    }
}