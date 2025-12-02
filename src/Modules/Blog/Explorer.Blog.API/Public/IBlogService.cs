using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public;

public interface IBlogService
{
    BlogDto CreateBlog(long userId, BlogCreateDto blogDto);
    List<BlogDto> GetUserBlogs(long userId);
    BlogDto UpdateBlog(long blogId, BlogUpdateDto blogDto);

    BlogDto PublishBlog(long blogId);
    BlogDto ArchiveBlog(long blogId);

    List<BlogDto> GetVisibleBlogs(long userId);    // lista blogova koje user sme da vidi

    CommentDto AddCommentToBlog(long blogId, long userId, CommentDto commentDto);
    CommentDto UpdateCommentInBlog(long blogId, long userId, CommentDto commentDto);
    public void DeleteCommentFromBlog(long blogId, long userId, long commentId);
}