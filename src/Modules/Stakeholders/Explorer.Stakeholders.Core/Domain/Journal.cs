using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum Status 
{
    Draft,
    Finished,      // privatno zavrseno (i dalje samo user vidi)
    Published      // public -> pojavi se kao Blog
}

public class Journal : Entity
{
    public string Title { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Status Status{ get; private set; }
    public double Latitude { get; private set; }    //za mapu
    public double Longitude { get; private set; }
    public string LocationName { get; private set; }
    public long UserId { get; private set; }
    public string? Content { get; private set; }
    public List<string> Images { get; private set; } = new();
    public List<string> Videos { get; private set; } = new();
    public long? PublishedBlogId { get; private set; }  //da veze za blog kad publishuje
    public List<JournalCollaborator> Collaborators { get; private set; } = new();

    protected Journal(){}
    public Journal(string? content, long userId, string title, double lat, double longit, string locationName )
    {
        UserId = userId;
        Title = title;
        CreatedAt = DateTime.UtcNow;
        Latitude = lat;
        Longitude = longit;
        LocationName = locationName;
        Status = Status.Draft;
        Content = content;
        Validate();
    }
    private void Validate()
    {
        if (UserId <= 0) throw new ArgumentException("UserId must be positive");
        if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Title can't be empty");
        if (string.IsNullOrWhiteSpace(LocationName)) throw new ArgumentException("Location can't be empty");
    }
    
    public void Finish()
    {
        Status = Status.Finished;
    }
    
    public void Update(string? content, string title)
    {
        Content = content;
        Title = title;
        Validate();
    }

    public void Publish(long blogId)
    {
        if (Status != Status.Draft)
            throw new InvalidOperationException("Only draft journals can be published.");

        if (PublishedBlogId != null)
            throw new InvalidOperationException("Journal is already published.");

        PublishedBlogId = blogId;
        Status = Status.Published;
    }

    public void SetMedia(List<string> images, List<string> videos)
    {
        Images = images ?? new();
        Videos = videos ?? new();
    }


    public bool IsOwner(long userId) => UserId == userId;
    public bool IsCollaborator(long userId) => Collaborators.Any(c => c.UserId == userId);

    public void AddCollaborator(long ownerId, long collaboratorUserId)
    {
        if (!IsOwner(ownerId)) throw new UnauthorizedAccessException("Only owner can manage collaborators.");
        if (collaboratorUserId == ownerId) throw new ArgumentException("Cannot add yourself as collaborator.");
        if (Collaborators.Any(c => c.UserId == collaboratorUserId)) throw new ArgumentException("User is already a collaborator.");

        Collaborators.Add(new JournalCollaborator(Id, collaboratorUserId));
    }

    public void RemoveCollaborator(long ownerId, long collaboratorUserId)
    {
        if (!IsOwner(ownerId)) throw new UnauthorizedAccessException("Only owner can manage collaborators.");

        var existing = Collaborators.FirstOrDefault(c => c.UserId == collaboratorUserId);
        if (existing == null) return;

        Collaborators.Remove(existing);
    }
}