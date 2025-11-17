namespace Explorer.Blog.API.Public;

public class BlogCreateDto
{
    // UserId ovde ne postoji zato sto se dobija iz JWT tokena u kontroleru, da useri ne bi fejkovali svoj ID; 
    public string Title {get; set;}
    public string Description {get; set;}
    public List<string>? Images {get; set;}
}