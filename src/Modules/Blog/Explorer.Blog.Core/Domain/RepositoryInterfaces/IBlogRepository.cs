namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IBlogRepository
{
    Blog Create(Blog blog);
    List<Blog> GetByUserId(long userId);
    Blog Update(Blog blog);
    Blog GetById(long id);
}