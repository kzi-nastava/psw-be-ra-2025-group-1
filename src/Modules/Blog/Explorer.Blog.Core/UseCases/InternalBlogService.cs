using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Internal;
using Explorer.Blog.API.Public;

namespace Explorer.Blog.Core.UseCases;

public class InternalBlogService : IInternalBlogService
{
    private readonly IBlogService _blogService;

    public InternalBlogService(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public BlogDto CreateFromJournal(long userId, string title, string? content)
    {
        var dto = new BlogCreateDto
        {
            Title = title,
            Description = content ?? "",
            Images = new List<string>(),
            Videos = new List<string>()
        };

        // 1) kreiraj draft blog
        var created = _blogService.CreateBlog(userId, dto);

        // 2) odmah publish
        var published = _blogService.PublishBlog(created.Id, userId);

        return published;
    }
}
