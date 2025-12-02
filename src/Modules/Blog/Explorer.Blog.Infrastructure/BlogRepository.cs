using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
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
        return _context.Blogs.Include(b => b.Comments).Where(b => b.UserId == userId).ToList(); // LINQ query to filter by userId
    }

    public BlogEntity Update(BlogEntity blog)
    {
        _context.Blogs.Update(blog);
        _context.SaveChanges(); // commits changes to Postgre
        return blog;
    }

    public BlogEntity GetById(long id)
    {
        return _context.Blogs.Include(b => b.Comments).FirstOrDefault(b => b.Id == id) ?? throw new KeyNotFoundException($"Blog with the ID {id} was not found.");
    }

    public List<BlogEntity> GetVisibleForUser(long userId)          //svi blogovi koji nisu draft + moji draftovi
    {
        return _context.Blogs
            .Include(b => b.Comments)
            .Where(b => b.Status != BlogStatus.Draft || b.UserId == userId)
            .ToList();
    }

    public List<Comment> GetCommentsForBlog(long blogId)
    {
        var blog = GetById(blogId);
        return blog.Comments;
    }

    public Comment GetCommentForBlog(long blogId, long commentId)
    {
        var blog = GetById(blogId);
        var comment = blog.Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} was not found in Blog with ID {blogId}.");
        }
        return comment;
    }

    public Comment AddCommentToBlog(long blogId, Comment comment)
    {
        var blog = GetById(blogId);
        blog.Comments.Add(comment);
        _context.SaveChanges();
        return comment;
    }

    public Comment UpdateCommentInBlog(long blogId, Comment updatedComment)
    {
        var blog = GetById(blogId);
        var existingComment = blog.UpdateComment(updatedComment);
        _context.SaveChanges();
        return existingComment;
    }

    public void DeleteComment(long blogId, long commentId)
    {
        var blog = GetById(blogId);
        blog.DeleteComment(commentId);
        _context.SaveChanges();
    }
}