using AutoMapper;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Domain.Blog, BlogDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images))
            .ForMember(d => d.Videos, opt => opt.MapFrom(s => s.Videos))
            .ForMember(d => d.Votes, opt => opt.MapFrom(s => s.Votes))
            .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments));

        CreateMap<Domain.Comment, CommentDto>().ReverseMap();
        CreateMap<Domain.Vote, VoteDto>().ReverseMap();
    }
}