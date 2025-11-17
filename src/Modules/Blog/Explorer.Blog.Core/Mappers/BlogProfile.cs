using AutoMapper;
using Explorer.Blog.API.Public;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Domain.Blog, BlogDto>().ReverseMap();
    }
}