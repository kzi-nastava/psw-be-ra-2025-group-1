namespace Explorer.Blog.API.Dtos
{
    public class CommentUpdateDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
    }
}
