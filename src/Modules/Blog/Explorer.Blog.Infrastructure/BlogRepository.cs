using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Infrastructure.Database;
using BlogEntity = Explorer.Blog.Core.Domain.Blog; // "Blog is a namespace but is used as a type" -> fix for this, not really a good one but whatever for now..

namespace Explorer.Blog.Infrastructure;

public class BlogRepository : IBlogRepository
{
    private readonly BlogContext _context;

    public BlogRepository(BlogContext context)
    {
        _context = context;
    }
    public BlogEntity Create(BlogEntity blog)
    {
        _context.Blogs.Add(blog);
        _context.SaveChanges();
        return blog;
    }
    
    public List<BlogEntity> GetByUserId(long userId)
    {
        return _context.Blogs.Where(b => b.UserId == userId).ToList(); // LINQ query to filter by userId
    }

    public BlogEntity Update(BlogEntity blog)
    {
        _context.Blogs.Update(blog);
        _context.SaveChanges(); // commits changes to Postgre
        return blog;
    }

    public BlogEntity GetById(long id)
    {
        return _context.Blogs.Find(id) ?? throw new KeyNotFoundException($"Blog with the ID {id} was not found.");
    }

    public List<BlogEntity> GetVisibleForUser(long userId)          //svi blogovi koji nisu draft + moji draftovi
    {
        return _context.Blogs
            .Where(b => b.Status != BlogStatus.Draft || b.UserId == userId)
            .ToList();
    }
}