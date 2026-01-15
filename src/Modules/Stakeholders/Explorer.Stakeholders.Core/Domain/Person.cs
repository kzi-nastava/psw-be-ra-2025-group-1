using Explorer.BuildingBlocks.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace Explorer.Stakeholders.Core.Domain;

public class Person : Entity
{
    public long UserId { get; init; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }

    [Column("ProfileImagePath")]
    public string? ProfileImageUrl { get; set; }
    public string? Biography { get; set; }
    public string? Quote { get; set; }

    public Person(long userId, string name, string surname, string email, string profileImageUrl = "", string biography = "", string quote = "")
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        Email = email;
        ProfileImageUrl = profileImageUrl;
        Biography = biography;
        Quote = quote;
        Validate();
    }

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid UserId");
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
        if (string.IsNullOrWhiteSpace(Surname)) throw new ArgumentException("Invalid Surname");
        if (!MailAddress.TryCreate(Email, out _)) throw new ArgumentException("Invalid Email");
    }
}