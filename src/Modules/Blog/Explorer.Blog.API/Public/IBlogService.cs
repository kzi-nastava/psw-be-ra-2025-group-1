namespace Explorer.Blog.API.Public;

public interface IBlogService
{
    BlogDto CreateBlog(BlogCreateDto blogDto);
    List<BlogDto> GetUserBlogs(long userId);
    BlogDto UpdateBlog(long blogId, BlogUpdateDto blogDto);
}