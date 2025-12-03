using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class Blog : AggregateRoot
{
    public long UserId {get; private set; }
    public string Title {get; private set;}
    public string Description {get; private set;}
    public DateTime CreationDate {get; private set;}
    public List<string> Images {get; private set;}
    public BlogStatus Status {get; private set;}
    public DateTime? LastModifiedDate {get; private set;}
    public List<Comment> Comments {get; private set;}

    public Blog(long userId, string title, string description, List<string>? images = null)
    {
        UserId = userId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        CreationDate = DateTime.UtcNow;
        Images = images ?? new List<string>();
        Status = BlogStatus.Draft; // Kada korisnik kreira blog, on se nalazi u stanju pripreme.
        Comments = new List<Comment>();
        Validate();
    }

    private void Validate() // Validacija polja koja ne bi trebalo da budu prazna
    {
        if (UserId <= 0) throw new ArgumentException("UserId must be positive");
        if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Title can't be empty");
        if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Description can't be empty");
    }

    public void Update(string title, string description, List<string>? images) // Updated Update logic
    {
        if (Status == BlogStatus.Archived || Status == BlogStatus.Closed)
        {
            throw new InvalidOperationException("Cannot update a blog that is archived or closed.");
        }

        if (Status == BlogStatus.Published) // "Autor može da ažurira samo opis za objavljen blog"
        {
            if (Title != title) throw new InvalidOperationException ("Cannot change the title of a published blog."); 

            Description = description;
        }
        else if (Status == BlogStatus.Draft) // Tokom pripreme autor može da podešava naziv, opis i slike bloga.
        {
            Title = title;
            Description = description;
            Images = images;
        }

        LastModifiedDate = DateTime.UtcNow; // Pamti se vreme poslednje izmene za svako ažuriranje
    }
    
    public void Publish() // U momentu kada je autor zadovoljan, može da objavi blog.
    {
        if (Status != BlogStatus.Draft)
        {
            throw new InvalidOperationException("Only drafts can be published");
        }
        Status = BlogStatus.Published;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Archive() // Autor može da arhivira blog koji je objavljen, nakon čega nije moguće menjati sadržaj.
    {
        if (Status != BlogStatus.Published)
        {
            throw new InvalidOperationException("Only published blogs can be archived.");
        }
        Status = BlogStatus.Archived;
        LastModifiedDate = DateTime.UtcNow;
    }

    public Comment AddComment(long userId, string content) // Dodavanje komentara na blog
    {
        if (Status != BlogStatus.Published)
        {
            throw new InvalidOperationException("Comments can only be added to published blogs.");
        }
        var comment = new Comment(userId, content);
        Comments.Add(comment);
        return comment;
    }

    public Comment UpdateComment(long userId, long commentId, string content) // Ažuriranje komentara na blog
    {
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);

        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
        }
        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("User is not authorized to update this comment.");
        }

        comment.UpdateContent(content);
        return comment;
    }

    public void DeleteComment(long commentId, long userId)
    {
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
        }
        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("User is not authorized to delete this comment.");
        }
        Comments.Remove(comment);
    }
}