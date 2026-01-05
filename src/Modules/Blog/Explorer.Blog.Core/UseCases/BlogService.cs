using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Blog.Core.UseCases;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;
    private readonly IInternalPersonService _personService;

    public BlogService(IBlogRepository blogRepository, IMapper mapper, IInternalPersonService personService)
    {
        _blogRepository = blogRepository;
        _mapper = mapper;
        _personService = personService;
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
        return MapBlogsForUser(blogs, userId);
    }

    public BlogDto UpdateBlog(long blogId, BlogUpdateDto blogDto)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.Update(blogDto.Title, blogDto.Description, blogDto.Images); // The Aggregate (Blog.cs) now throws an exception if this is Invalid.
        var updatedBlog = _blogRepository.Update(blog);
        return AddAuthorToComments(updatedBlog);
    }

    public BlogDto PublishBlog(long  blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.Publish(); // Changes the status of the blog to Published
        var updatedBlog = _blogRepository.Update(blog);
        return AddAuthorToComments(updatedBlog);
    }
     
     public BlogDto ArchiveBlog(long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.Archive(); // Changes the status of the blog to Archived
        var updatedBlog = _blogRepository.Update(blog);
        return AddAuthorToComments(updatedBlog);
    }

    public List<BlogDto> GetVisibleBlogs(long userId)
    {
        var blogs = _blogRepository.GetVisibleForUser(userId);
        return MapBlogsForUser(blogs, userId);
    }

    public CommentDto GetCommentForBlog(long blogId, long commentId)
    {
        var comment = _blogRepository.GetCommentForBlog(blogId, commentId);
        return AddAuthorToComment(comment);
    }

    public CommentDto AddCommentToBlog(long blogId, CommentCreateDto commentDto)
    {
        var comment = _blogRepository.AddCommentToBlog(blogId, commentDto.UserId, commentDto.Content);
        return AddAuthorToComment(comment);
    }

    public CommentDto UpdateCommentInBlog(long blogId, CommentUpdateDto commentDto)
    {
        var comment = _blogRepository.UpdateCommentInBlog(blogId, commentDto.UserId, commentDto.Id, commentDto.Content);
        return AddAuthorToComment(comment);
    }

    public void DeleteCommentFromBlog(long blogId, long userId, long commentId)
    {
        var comment = _blogRepository.GetCommentForBlog(blogId, commentId);
        _blogRepository.DeleteComment(blogId, userId, commentId);
    }

    private List<BlogDto> AddAuthorToComments(List<Domain.Blog> blogs)
    {
        var mappedBlogs = _mapper.Map<List<BlogDto>>(blogs);
        foreach (var blog in mappedBlogs)
        {
            foreach (var comment in blog.Comments)
            {
                var person = _personService.GetPersonByUserId(comment.UserId);
                comment.AuthorName = person.Name + " " + person.Surname;
            }
        }
        return mappedBlogs;
    }

    private BlogDto AddAuthorToComments(Domain.Blog blog)
    {
        var mappedBlog = _mapper.Map<BlogDto>(blog);
        foreach (var comment in mappedBlog.Comments)
        {
            var person = _personService.GetPersonByUserId(comment.UserId);
            comment.AuthorName = person.Name + " " + person.Surname;
        }
        return mappedBlog;
    }

    private CommentDto AddAuthorToComment(Domain.Comment comment)
    {
        var mappedComment = _mapper.Map<CommentDto>(comment);
        var person = _personService.GetPersonByUserId(mappedComment.UserId);
        mappedComment.AuthorName = person.Name + " " + person.Surname;
        return mappedComment;
    }

    public VoteDto? AddVoteToBlog(long blogId, long userId, VoteCreateDto voteDto)
    {
        var blog = _blogRepository.GetById(blogId);

        if (!Enum.TryParse<VoteType>(voteDto.VoteType, true, out var voteType))
            throw new ArgumentException("Invalid vote type. Use 'Upvote' or 'Downvote'.");

        var vote = blog.AddVote(userId, voteType);

        _blogRepository.Update(blog);
        return vote == null ? null : _mapper.Map<VoteDto>(vote);
    }

    public void RemoveVoteFromBlog(long blogId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.RemoveVote(userId);
        _blogRepository.Update(blog);
    }

    public BlogDto GetBlogById(long blogId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);
    
        // Check if user can view this blog
        if (blog.Status == BlogStatus.Draft && blog.UserId != userId)
            throw new UnauthorizedAccessException("You can only view draft blogs you created.");

        return MapBlogForUser(blog, userId);
    }

    private BlogDto MapBlogForUser(Domain.Blog blog, long userId)
    {
        var dto = AddAuthorToComments(blog); // vec mapira i doda authorName na komentare

        dto.VoteScore = blog.GetVoteScore();

        var myVote = blog.Votes.FirstOrDefault(v => v.UserId == userId);
        dto.CurrentUserVote = myVote == null
            ? null
            : (myVote.VoteType == VoteType.Upvote ? VoteTypeDto.Upvote : VoteTypeDto.Downvote);

        return dto;
    }

    private List<BlogDto> MapBlogsForUser(List<Domain.Blog> blogs, long userId)
    {
        return blogs.Select(b => MapBlogForUser(b, userId)).ToList();
    }

}