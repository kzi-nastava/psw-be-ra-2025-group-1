using System.IO.Compression;
using System.Linq.Expressions;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize] // Autentikacija da samo autori/turisti sa validnim JWT tokenima mogu da pristupe
[Route("api/blog")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpPost]
    public ActionResult<BlogDto> CreateBlog([FromBody] BlogCreateDto blogDto) // Kreiranje bloga
    {
        var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value ?? "0");
        var result = _blogService.CreateBlog(userId, blogDto);

        return Ok(result);
    }

    [HttpGet("my")]
    public ActionResult<List<BlogDto>> GetMyBlogs() // Pregled blogova; auth token sadrzi userId -> JWT
    {
        var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value ?? "0");
        var result = _blogService.GetUserBlogs(userId);

        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<BlogDto>> GetVisibleBlogs()        //pregled svih blogova koje user sme da vidi
    {
        var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value ?? "0");
        var result = _blogService.GetVisibleBlogs(userId);

        return Ok(result);
    }


    [HttpPut("{id}")]
    public ActionResult<BlogDto> UpdateBlog(long id, [FromBody] BlogUpdateDto blogDto) // Editovanje bloga
    {
        try
        {
            var result = _blogService.UpdateBlog(id, blogDto);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}/publish")]
    public ActionResult<BlogDto> Publish(long id)
    {
        try
        {
            var result = _blogService.PublishBlog(id);
            return Ok(result);
        }
     catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}/archive")]
    public ActionResult<BlogDto> Archive(long id)
    {
        try
        {
            var result = _blogService.ArchiveBlog(id);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("comments/get/blogId={blogId}/commentId={commentId}")]
    public ActionResult<CommentDto> GetComment(long blogId, long commentId)
    {
        var result = _blogService.GetCommentForBlog(blogId, commentId);
        return Ok(result);
    }

    [HttpPut("comments/add/blogId={blogId}")]
    public ActionResult<CommentDto> AddComment(long blogId, [FromBody] CommentCreateDto commentDto) 
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id") is not null ? long.Parse(User.Claims.First(c => c.Type == "id").Value) : -1;
        commentDto.UserId = userId;
        var result = _blogService.AddCommentToBlog(blogId, commentDto);

        return Ok(result);
    }

    [HttpPut("comments/update/blogId={blogId}/commentId={commentId}")]
    public ActionResult<CommentDto> UpdateComment(long blogId, long commentId, [FromBody] CommentUpdateDto commentDto)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id") is not null ? long.Parse(User.Claims.First(c => c.Type == "id").Value) : -1;
        commentDto.UserId = userId;
        commentDto.Id = commentId;
        var result = _blogService.UpdateCommentInBlog(blogId, commentDto);

        return Ok(result);
    }

    [HttpDelete("comments/delete/blogId={blogId}/commentId={commentId}")]
    public ActionResult DeleteComment(long blogId, long commentId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id") is not null ? long.Parse(User.Claims.First(c => c.Type == "id").Value) : -1;
        _blogService.DeleteCommentFromBlog(blogId, userId, commentId);
        return Ok();
    }
}