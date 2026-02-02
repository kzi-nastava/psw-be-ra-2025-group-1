using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Internal;

public interface IInternalBlogService
{
    BlogDto CreateFromJournal(long userId, string title, string? content);   //most da journals pozove blog i kaze "napravi blog od ovih vrednosti"
}
