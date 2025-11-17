using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class Blog : Entity
{
    public string Title {get; private set;}
    public string Description {get; private set;}
    public DateTime CreationDate {get; private set;}
    public List<string> Images {get; private set;}

    public Blog(string title, string description, List<string>? images = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        CreationDate = DateTime.UtcNow;
        Images = images ?? new List<string>();
        Validate();
    }

    private void Validate() // Validacija polja koja ne bi trebalo da budu prazna
    {
        if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Title can't be empty");
        if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Description can't be empty");
    }

    public void Update(string title, string description, List<string>? images) // Korisnik moze izmeniti naslov, opis i slike
    {
        Title = title ?? Title;
        Description = description ?? Description;
        Images = images ?? Images;
        Validate();
    }
    
}