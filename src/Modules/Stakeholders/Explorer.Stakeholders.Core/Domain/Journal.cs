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
}