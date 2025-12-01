namespace Explorer.Blog.API.Public;

public interface IBlogService
{
    BlogDto CreateBlog(long userId, BlogCreateDto blogDto);
    List<BlogDto> GetUserBlogs(long userId);
    BlogDto UpdateBlog(long blogId, BlogUpdateDto blogDto);

    BlogDto PublishBlog(long blogId);
    BlogDto ArchiveBlog(long blogId);
}