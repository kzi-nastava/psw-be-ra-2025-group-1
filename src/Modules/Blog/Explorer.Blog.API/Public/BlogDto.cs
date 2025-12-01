namespace Explorer.Blog.API.Public;

public class BlogDto
{
    public long Id {get; set;}
    public long UserId {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    public DateTime CreationDate {get; set;}
    public List<string> Images {get; set;}
    public string Status {get; set;}
    public DateTime? LastModifiedDate {get; set;}
}