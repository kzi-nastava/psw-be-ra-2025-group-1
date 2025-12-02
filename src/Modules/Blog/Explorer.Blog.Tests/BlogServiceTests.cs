using AutoMapper;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Mappers;
using Explorer.Blog.Core.UseCases;
using BlogEntity = Explorer.Blog.Core.Domain.Blog;

using Moq;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests;

public class BlogServiceTests
{
    private readonly Mock<IBlogRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly BlogService _service;

    public BlogServiceTests()
    {
        // AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BlogProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _mockRepository = new Mock<IBlogRepository>();

        _service = new BlogService(_mockRepository.Object, _mapper);
    }

    [Fact]
    public void CreateBlog_ValidInput_ReturnsCreatedBlog()
    {
        var userId = 1L;
        var createDto = new BlogCreateDto
        {
            Title = "Test Blog",
            Description = "Test description",
            Images = new List<string> { "https://example.com/image.jpg" }
        };

        var createdBlog = new Core.Domain.Blog(userId, createDto.Title, createDto.Description, createDto.Images);
        
        _mockRepository
            .Setup(repo => repo.Create(It.IsAny<Core.Domain.Blog>()))
            .Returns(createdBlog);

        var result = _service.CreateBlog(userId, createDto);

        result.ShouldNotBeNull();
        result.Title.ShouldBe("Test Blog");
        result.Description.ShouldBe("Test description");
        result.UserId.ShouldBe(userId);
        result.Images.Count.ShouldBe(1);
        
        _mockRepository.Verify(repo => repo.Create(It.IsAny<Core.Domain.Blog>()), Times.Once);
    }

    [Fact]
    public void CreateBlog_WithoutImages_ReturnsCreatedBlog()
    {
        var userId = 2L;
        var createDto = new BlogCreateDto
        {
            Title = "Blog Without Images",
            Description = "Description here",
            Images = null
        };

        var createdBlog = new Core.Domain.Blog(userId, createDto.Title, createDto.Description, null);
        
        _mockRepository
            .Setup(repo => repo.Create(It.IsAny<Core.Domain.Blog>()))
            .Returns(createdBlog);

        var result = _service.CreateBlog(userId, createDto);

        result.ShouldNotBeNull();
        result.Images.ShouldBeEmpty();
    }

    [Fact]
    public void GetUserBlogs_UserHasBlogs_ReturnsBlogList()
    {
        var userId = 1L;
        var blogs = new List<Core.Domain.Blog>
        {
            new Core.Domain.Blog(userId, "Blog 1", "Description 1", null),
            new Core.Domain.Blog(userId, "Blog 2", "Description 2", null)
        };

        _mockRepository
            .Setup(repo => repo.GetByUserId(userId))
            .Returns(blogs);

        var result = _service.GetUserBlogs(userId);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Title.ShouldBe("Blog 1");
        result[1].Title.ShouldBe("Blog 2");
        
        _mockRepository.Verify(repo => repo.GetByUserId(userId), Times.Once);
    }

    [Fact]
    public void GetUserBlogs_UserHasNoBlogs_ReturnsEmptyList()
    {
        var userId = 999L;
        
        _mockRepository
            .Setup(repo => repo.GetByUserId(userId))
            .Returns(new List<Core.Domain.Blog>());

        var result = _service.GetUserBlogs(userId);

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void UpdateBlog_ValidInput_ReturnsUpdatedBlog()
    {
        var blogId = 1L;
        var userId = 1L;
        var existingBlog = new Core.Domain.Blog(userId, "Old Title", "Old Description", null);
        
        var updateDto = new BlogUpdateDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Images = new List<string> { "https://example.com/new-image.jpg" }
        };

        _mockRepository
            .Setup(repo => repo.GetById(blogId))
            .Returns(existingBlog);

        _mockRepository
            .Setup(repo => repo.Update(It.IsAny<Core.Domain.Blog>()))
            .Returns((Core.Domain.Blog b) => b);

        var result = _service.UpdateBlog(blogId, updateDto);

        result.ShouldNotBeNull();
        result.Title.ShouldBe("Updated Title");
        result.Description.ShouldBe("Updated Description");
        result.Images.Count.ShouldBe(1);
        
        _mockRepository.Verify(repo => repo.GetById(blogId), Times.Once);
        _mockRepository.Verify(repo => repo.Update(It.IsAny<Core.Domain.Blog>()), Times.Once);
    }

    [Fact]
    public void UpdateBlog_BlogNotFound_ThrowsException()
    {
        var blogId = 999L;
        var updateDto = new BlogUpdateDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Images = null
        };

        _mockRepository
            .Setup(repo => repo.GetById(blogId))
            .Throws(new KeyNotFoundException($"Blog with ID {blogId} not found"));

        Should.Throw<KeyNotFoundException>(() => 
            _service.UpdateBlog(blogId, updateDto)
        );
    }

    [Fact]
    public void GetVisibleBlogs_FiltersOutOtherUsersDrafts()
    {
        var userId = 1L;

        var myDraft = new BlogEntity(userId, "My draft", "desc", null);   // konstruktor postavlja Status = Draft

        var myPublished = new BlogEntity(userId, "My published", "desc", null);
        myPublished.Publish();   // kroz metodu menjamo u Published

        var otherDraft = new BlogEntity(2, "Other draft", "desc", null);   // Draft po defaultu

        var blogs = new List<BlogEntity> { myDraft, myPublished, otherDraft };

        _mockRepository
            .Setup(r => r.GetVisibleForUser(userId))
            .Returns(blogs.Where(b => b.Status != BlogStatus.Draft || b.UserId == userId).ToList());

        var result = _service.GetVisibleBlogs(userId);

        result.Count.ShouldBe(2);
        result.Any(b => b.Title == "Other draft").ShouldBeFalse();
    }

}