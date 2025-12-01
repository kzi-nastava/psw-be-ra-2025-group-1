using AutoMapper;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;

    public BlogService(IBlogRepository blogRepository, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _mapper = mapper;
    }

    public BlogDto CreateBlog(long userId, BlogCreateDto blogDto)
    {
        var blog = new Domain.Blog(userId, blogDto.Title, blogDto.Description, blogDto.Images);
        var createdBlog = _blogRepository.Create(blog);
        return _mapper.Map<BlogDto>(createdBlog);
    }

    public List<BlogDto> GetUserBlogs(long userId)
    {
        var blogs = _blogRepository.GetByUserId(userId);
        return _mapper.Map<List<BlogDto>>(blogs);
    }

    public BlogDto UpdateBlog(long blogId, BlogUpdateDto blogDto)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.Update(blogDto.Title, blogDto.Description, blogDto.Images); // The Aggregate (Blog.cs) now throws an exception if this is Invalid.
        var updatedBlog = _blogRepository.Update(blog);
        return _mapper.Map<BlogDto>(updatedBlog);
    }

    public BlogDto PublishBlog(long  blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.Publish(); // Changes the status of the blog to Published
        var updatedBlog = _blogRepository.Update(blog);
        return _mapper.Map<BlogDto>(updatedBlog);
    }
     
     public BlogDto ArchiveBlog(long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.Archive(); // Changes the status of the blog to Archived
        var updatedBlog = _blogRepository.Update(blog);
        return _mapper.Map<BlogDto>(updatedBlog);
    }
}