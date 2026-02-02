namespace Explorer.Stakeholders.API.Dtos;

public enum Status 
{
    Draft,
    Finished,
    Published
}

public class JournalDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status{ get; set; }
    public string? Content { get; set; }

    public string LocationName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<string> Images { get; set; } = new();
    public List<string> Videos { get; set; } = new();

    public long? PublishedBlogId { get; set; }

    public List<CollaboratorDto> Collaborators { get; set; } = new();
    public bool CanManageCollaborators { get; set; }   // owner-only
    public bool CanEdit { get; set; }                  // owner or collaborator
    public bool CanDelete { get; set; }                // owner-only
}