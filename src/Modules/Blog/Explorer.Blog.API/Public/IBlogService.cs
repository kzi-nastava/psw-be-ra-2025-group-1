using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public;

public interface IBlogService
{
    BlogDto CreateBlog(long userId, BlogCreateDto blogDto);
    List<BlogDto> GetUserBlogs(long userId);
    BlogDto UpdateBlog(long blogId, BlogUpdateDto blogDto);

    BlogDto PublishBlog(long blogId);
    BlogDto ArchiveBlog(long blogId);

    void DeleteBlog(long blogId, long userId);

    List<BlogDto> GetVisibleBlogs(long userId);    // lista blogova koje user sme da vidi

    public CommentDto GetCommentForBlog(long blogId, long commentId);
    CommentDto AddCommentToBlog(long blogId, CommentCreateDto commentDto);
    CommentDto UpdateCommentInBlog(long blogId, CommentUpdateDto commentDto);
    public void DeleteCommentFromBlog(long blogId, long userId, long commentId);

    VoteDto AddVoteToBlog(long blogId, long userId, VoteCreateDto voteDto);
    void RemoveVoteFromBlog(long blogId, long userId);

    BlogDto GetBlogById(long blogId, long userId);
}