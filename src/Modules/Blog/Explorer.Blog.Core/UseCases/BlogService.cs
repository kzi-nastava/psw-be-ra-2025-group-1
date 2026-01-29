using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;
    private readonly IInternalPersonService _personService;
    private readonly IInternalUserService _users;
    private readonly IJournalRepository _journalRepo;

    public BlogService(IBlogRepository blogRepository, IMapper mapper, IInternalPersonService personService, IInternalUserService users, IJournalRepository journalRepo)
    {
        _blogRepository = blogRepository;
        _mapper = mapper;
        _personService = personService;
        _users = users;
        _journalRepo = journalRepo;
    }

    public BlogDto CreateBlog(long userId, BlogCreateDto blogDto)
    {
        var blog = new Domain.Blog(userId, blogDto.Title, blogDto.Description, blogDto.Images, blogDto.Videos);
        var createdBlog = _blogRepository.Create(blog);
        return _mapper.Map<BlogDto>(createdBlog);
    }

    public List<BlogDto> GetUserBlogs(long userId)
    {
        var blogs = _blogRepository.GetByUserId(userId);
        return MapBlogsForUser(blogs, userId);
    }

    public BlogDto UpdateBlog(long blogId, BlogUpdateDto blogDto, long currentUserId)
    {
        var blog = _blogRepository.GetById(blogId);

        if (!(blog.IsOwner(currentUserId) || blog.IsCollaborator(currentUserId)))
            throw new UnauthorizedAccessException("Nemate dozvolu da menjate ovaj blog.");

        blog.Update(blogDto.Title, blogDto.Description, blogDto.Images, blogDto.Videos); // The Aggregate (Blog.cs) now throws an exception if this is Invalid.

        var updatedBlog = _blogRepository.Update(blog);
        return MapBlogForUser(updatedBlog, currentUserId);
    }

    public BlogDto PublishBlog(long  blogId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);

        if (!blog.IsOwner(userId))
            throw new UnauthorizedAccessException("Nemate dozvolu.");

        blog.Publish(); // Changes the status of the blog to Published
        var updatedBlog = _blogRepository.Update(blog);
        return MapBlogForUser(updatedBlog, userId);
    }
     
     public BlogDto ArchiveBlog(long blogId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);

        if (!blog.IsOwner(userId))
            throw new UnauthorizedAccessException("Nemate dozvolu.");

        blog.Archive(); // Changes the status of the blog to Archived
        var updatedBlog = _blogRepository.Update(blog);
        return MapBlogForUser(updatedBlog, userId);
    }

    public void DeleteBlog(long blogId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);

        if (!blog.IsOwner(userId))
            throw new UnauthorizedAccessException("You can only delete your own blog.");

        _blogRepository.Delete(blogId);
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

        // BLOG collaborators
        var blogIds = blog.Collaborators.Select(c => c.UserId).Distinct().ToList();
        var blogMap = _users.GetUsernamesByIds(blogIds);

        var blogCollabs = blog.Collaborators.Select(c => new BlogCollaboratorDto
        {
            UserId = c.UserId,
            Username = blogMap.TryGetValue(c.UserId, out var uname) ? uname : ""
        }).ToList();

        // JOURNAL collaborators (ako je blog iz journala)
        var journal = _journalRepo.GetByPublishedBlogId(blog.Id);

        var journalCollabs = new List<BlogCollaboratorDto>();
        if (journal != null)
        {
            journalCollabs = journal.Collaborators.Select(c => new BlogCollaboratorDto
            {
                UserId = c.UserId,
                Username = c.User?.Username ?? ""
            }).ToList();
        }

        dto.Collaborators = blogCollabs
            .Concat(journalCollabs)
            .GroupBy(c => c.UserId)
            .Select(g => g.First())
            .ToList();

        var isBlogOwner = blog.UserId == userId;
        var isBlogCollab = blog.Collaborators.Any(c => c.UserId == userId);
        var isJournalCollab = journal != null && journal.Collaborators.Any(c => c.UserId == userId);

        dto.CanEdit = isBlogOwner || isBlogCollab || isJournalCollab;

        dto.CanManageCollaborators = isBlogOwner;

        return dto;
    }

    private List<BlogDto> MapBlogsForUser(List<Domain.Blog> blogs, long userId)
    {
        return blogs.Select(b => MapBlogForUser(b, userId)).ToList();
    }

    public BlogDto AddCollaborator(long ownerId, long blogId, string collaboratorUsername)
    {
        var blog = _blogRepository.GetById(blogId);

        var collaboratorId = _users.GetUserIdByUsername(collaboratorUsername);
        if (collaboratorId == null) throw new ArgumentException("User does not exist.");

        blog.AddCollaborator(ownerId, collaboratorId.Value);

        var updated = _blogRepository.Update(blog);
        return MapBlogForUser(updated, ownerId); 
    }

    public BlogDto RemoveCollaborator(long ownerId, long blogId, long collaboratorUserId)
    {
        var blog = _blogRepository.GetById(blogId);

        blog.RemoveCollaborator(ownerId, collaboratorUserId);

        var updated = _blogRepository.Update(blog);
        return MapBlogForUser(updated, ownerId);
    }

}