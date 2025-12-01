using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{

    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<TourPreference> TourPreferences { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<Rating> Ratings { get; set; }
    public DbSet<UserLocation> UserLocations { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}


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


        ConfigureMessaging(modelBuilder);



        modelBuilder.Entity<Rating>(cfg =>          //fluent mapiranja za Rating
        {
            cfg.ToTable("Ratings");
            cfg.HasKey(r => r.Id);

            cfg.Property(r => r.UserId).IsRequired();
            cfg.Property(r => r.Score).IsRequired();          // 1–5 validacija je u domenu, ovde samo Required
            cfg.Property(r => r.Comment).HasMaxLength(500);   // opcionalno
            cfg.Property(r => r.CreatedAt).IsRequired();
            cfg.Property(r => r.UpdatedAt);

            cfg.HasIndex(r => r.UserId);
            cfg.HasIndex(r => r.CreatedAt);
        });
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

        modelBuilder.Entity<TourPreference>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<TourPreference>(s => s.UserId);
    }
}