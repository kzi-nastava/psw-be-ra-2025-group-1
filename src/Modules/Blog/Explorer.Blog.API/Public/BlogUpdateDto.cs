namespace Explorer.Blog.API.Public;

public class BlogUpdateDto
{
    public string Title {get; set;}
    public string Description {get; set;}
    public List<string>? Images {get; set;}
}