namespace Explorer.Blog.API.Dtos
{
    public class CommentCreateDto
    {
        public long UserId { get; set; }
        public string Content { get; set; }
    }
}
