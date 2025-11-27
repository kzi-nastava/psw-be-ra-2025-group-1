using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum Status 
{
    Draft,
    Finished
}

public class Journal : Entity
{
    public string Title { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Status Status{ get; private set; }
    public string Location { get; private set; }
    public long UserId { get; private set; }
    public string? Content { get; private set; }
    
    protected Journal(){}
    public Journal(string? content, long userId, string title, string location )
    {
        UserId = userId;
        Title = title;
        CreatedAt = DateTime.UtcNow;
        Location = location;
        Status = Status.Draft;
        Content = content;
        Validate();
    }
    private void Validate()
    {
        if (UserId <= 0) throw new ArgumentException("UserId must be positive");
        if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Title can't be empty");
        if (string.IsNullOrWhiteSpace(Location)) throw new ArgumentException("Location can't be empty");
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

}