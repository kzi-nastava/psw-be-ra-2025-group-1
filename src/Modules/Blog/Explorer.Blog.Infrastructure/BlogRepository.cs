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
        return _context.Blogs.Include(b => b.Comments).Include(b => b.Votes).Where(b => b.UserId == userId).ToList(); // LINQ query to filter by userId
    }

    public BlogEntity Update(BlogEntity blog)
    {
        var existingVoteIds = _context.Votes.Where(v => v.BlogId == blog.Id).Select(v => v.Id).ToList();

        var currentVoteIds = blog.Votes.Select(v => v.Id).ToList();

        var votesToDelete = existingVoteIds.Except(currentVoteIds).ToList();

        foreach (var voteId in votesToDelete)
        {
            var voteToDelete = _context.Votes.Find(voteId);
            if (voteToDelete != null)
            {
                _context.Votes.Remove(voteToDelete);
            }
        }

        _context.Blogs.Update(blog);
        _context.SaveChanges(); // commits changes to Postgre
        return blog;
    }

    public void Delete(long blogId)
    {
        var blog = GetById(blogId); // ukljucuje Comments/Votes zbog Include
        _context.Blogs.Remove(blog);
        _context.SaveChanges();
    }


    public BlogEntity GetById(long id)
    {
        return _context.Blogs.Include(b => b.Comments).Include(b => b.Votes).FirstOrDefault(b => b.Id == id) ?? throw new KeyNotFoundException($"Blog with the ID {id} was not found.");
    }

    public List<BlogEntity> GetVisibleForUser(long userId)          //svi blogovi koji nisu draft + moji draftovi
    {
        return _context.Blogs
            .Include(b => b.Comments)
            .Include(b => b.Votes)
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

    public Comment AddCommentToBlog(long blogId, long userId, string content)
    {
        var blog = GetById(blogId);
        var comment = blog.AddComment(userId, content);
        _context.SaveChanges();
        return comment;
    }

    public Comment UpdateCommentInBlog(long blogId, long userId, long commentId, string content)
    {
        var blog = GetById(blogId);
        var existingComment = blog.UpdateComment(userId, commentId, content);
        _context.SaveChanges();
        return existingComment;
    }

    public void DeleteComment(long blogId, long userId, long commentId)
    {
        var blog = GetById(blogId);
        blog.DeleteComment(commentId, userId);
        _context.SaveChanges();
    }
}