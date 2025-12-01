using System.IO.Compression;
using System.Linq.Expressions;
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
        var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
        var result = _blogService.CreateBlog(userId, blogDto);

        return Ok(result);
    }

    [HttpGet("my")]
    public ActionResult<List<BlogDto>> GetMyBlogs() // Pregled blogova; auth token sadrzi userId -> JWT
    {
        var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
        var result = _blogService.GetUserBlogs(userId);

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
}