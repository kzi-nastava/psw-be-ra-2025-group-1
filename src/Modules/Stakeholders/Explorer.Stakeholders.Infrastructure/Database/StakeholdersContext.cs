using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public StakeholdersContext(DbContextOptions<StakeholdersContext> options)
        : base(options) { }

    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sve tabele u šemi stakeholders
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<Message>().ToTable("Messages");
        modelBuilder.Entity<Conversation>().ToTable("Conversations");
        modelBuilder.Entity<Person>().ToTable("People");
        modelBuilder.Entity<User>().ToTable("Users");

        // Dodaj ostale konfiguracije ako su potrebne
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        // Konfiguracija za messaging ako je potrebna
        ConfigureMessaging(modelBuilder);
    }

    private static void ConfigureMessaging(ModelBuilder modelBuilder)
    {
        // Conversation → Messages
        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message → Sender
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Message → Receiver
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Person → User relacija
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }
}