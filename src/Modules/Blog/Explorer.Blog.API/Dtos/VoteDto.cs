namespace Explorer.Blog.API.Dtos;

public class VoteDto
{
    public long Id { get ; set; }
    public long BlogId { get ; set; }
    public long UserId { get ; set; }
    public string VoteType { get ; set; }
    public DateTime CreationTime { get ; set; }
}

public class VoteCreateDto
{
    public string VoteType { get ; set; }
}