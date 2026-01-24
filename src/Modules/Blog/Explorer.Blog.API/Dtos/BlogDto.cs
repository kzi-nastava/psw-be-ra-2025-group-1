namespace Explorer.Blog.API.Dtos;

public class BlogDto
{
    public long Id {get; set;}
    public long UserId {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    public DateTime CreationDate {get; set;}
    public List<string> Images {get; set;}
    public List<string> Videos {get; set;}
    public string Status {get; set;}
    public DateTime? LastModifiedDate {get; set;}
    public List<CommentDto> Comments {get; set;} = new List<CommentDto>();

    public List<VoteDto> Votes { get ; set; }
    public int VoteScore { get ; set; }
    public VoteTypeDto? CurrentUserVote { get ; set; }
}