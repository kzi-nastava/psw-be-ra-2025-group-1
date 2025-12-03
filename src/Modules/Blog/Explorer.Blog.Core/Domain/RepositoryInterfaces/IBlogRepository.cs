namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IBlogRepository
{
    Blog Create(Blog blog);
    List<Blog> GetByUserId(long userId);
    Blog Update(Blog blog);
    Blog GetById(long id);
    List<Blog> GetVisibleForUser(long userId);          //svi blogovi koje trenutni korisnik sme da vidi
    List<Comment> GetCommentsForBlog(long blogId);
    Comment GetCommentForBlog(long blogId, long commentId);
    Comment AddCommentToBlog(long blogId, long userId, string content);
    public Comment UpdateCommentInBlog(long blogId, long userId, long commentId, string content);
    void DeleteComment(long blogId, long userId, long commentId);
}