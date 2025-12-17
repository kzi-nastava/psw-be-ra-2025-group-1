using System.Security.Cryptography.X509Certificates;
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

    public List<Vote> Votes { get; private set;} = new();

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
        if (Status == BlogStatus.Draft || Status == BlogStatus.Closed || Status == BlogStatus.Archived)
        {
            throw new InvalidOperationException("Comments can only be added to active or published blogs.");
        }
        var comment = new Comment(userId, content);
        Comments.Add(comment);

        UpdateStatusByEngagement();

        return comment;
    }

    public Comment UpdateComment(long userId, long commentId, string content) // Ažuriranje komentara na blog
    {
        if (Status == BlogStatus.Closed)
            throw new InvalidOperationException("Cannot modify comments on a closed blog.");

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
        if (Status == BlogStatus.Closed)
            throw new InvalidOperationException("Cannot delete comments on a closed blog.");

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

        UpdateStatusByEngagement();
    }

    public Vote AddVote(long userId, VoteType voteType)  
    {
        if (!IsInteractive())
        {
            throw new InvalidOperationException("Only published blogs can receive votes.");
        }

        // Remove the previous vote if a user tries to vote again after already having a vote 
        // Can't both upvote and downvnote a blog
        var existingVote = Votes.FirstOrDefault(v => v.UserId == userId);
        if (existingVote != null)
        {
            Votes.Remove(existingVote);
        }

        var vote = new Vote(userId, voteType);
        Votes.Add(vote);

        UpdateStatusByEngagement();

        return vote;
    }

    public void RemoveVote(long userId)
    {
        if (IsReadOnly())
        {
            throw new InvalidOperationException("Votes cannot be changed on a closed blog.");
        }

        var vote = Votes.FirstOrDefault(v => v.UserId == userId);
        if (vote != null)
        {
            Votes.Remove(vote);
            UpdateStatusByEngagement();
        }
    }

    // @Kristina Funkcija da uzme skor, za ono <-10 , >100, >500 sto treba da uradis
    public int GetVoteScore()
    {
        return Votes.Sum(v => (int)v.VoteType);
    }


    private void UpdateStatusByEngagement()
    {
        // Draft/Archived/Closed ne diramo ovde – rucno se prelazi u ta stanja
        if (Status == BlogStatus.Draft || Status == BlogStatus.Archived || Status == BlogStatus.Closed)
        {
            return;
        }

        var score = GetVoteScore();
        var commentsCount = Comments.Count;

        if (score < -10)
        {
            Status = BlogStatus.Closed;
        }
        else if (score > 500 && commentsCount > 30)
        {
            Status = BlogStatus.Famous;
        }
        else if (score > 100 || commentsCount > 10)
        {
            Status = BlogStatus.Active;
        }
        else
        {
            Status = BlogStatus.Published;      
        }

        LastModifiedDate = DateTime.UtcNow;
    }


    private bool IsReadOnly()
    {
        return Status == BlogStatus.Closed || Status == BlogStatus.Archived;
    }

    private bool IsInteractive()
    {
        return Status == BlogStatus.Published
            || Status == BlogStatus.Active
            || Status == BlogStatus.Famous;
    }

}