namespace Explorer.Stakeholders.API.Dtos;

public class JournalCollaboratorInfoDto
{
    public long UserId { get; set; }
    public string Username { get; set; } = "";
}

public class JournalBlogLinkDto
{
    public long JournalId { get; set; }
    public long BlogId { get; set; }
    public List<JournalCollaboratorInfoDto> Collaborators { get; set; } = new();
}
