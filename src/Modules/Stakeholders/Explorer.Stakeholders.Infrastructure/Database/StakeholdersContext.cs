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
    public DbSet<Journal> Journals { get; set; }
    public DbSet<JournalCollaborator> JournalCollaborators { get; set; }

    public DbSet<UserLocation> UserLocations { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemMessage> ProblemMessages { get; set; }
    public DbSet<Notification> Notifications { get; set; }

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

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();


        ConfigureMessaging(modelBuilder);

        modelBuilder.Entity<Rating>(cfg =>          
        {
            cfg.ToTable("Ratings");
            cfg.HasKey(r => r.Id);

            cfg.Property(r => r.UserId).IsRequired();
            cfg.Property(r => r.Score).IsRequired();          // 1–5 validacija je u domenu
            cfg.Property(r => r.Comment).HasMaxLength(500);   
            cfg.Property(r => r.CreatedAt).IsRequired();
            cfg.Property(r => r.UpdatedAt);

            cfg.HasIndex(r => r.UserId);
            cfg.HasIndex(r => r.CreatedAt);
        });
        
        modelBuilder.Entity<JournalCollaborator>()
            .ToTable("JournalCollaborators");

        modelBuilder.Entity<JournalCollaborator>(cfg =>
        {
            cfg.ToTable("JournalCollaborators");

            cfg.HasKey(x => new { x.JournalId, x.UserId }); 

            cfg.HasIndex(x => new { x.JournalId, x.UserId }).IsUnique();

            cfg.HasOne(x => x.User)
               .WithMany()                 // User nema kolekciju Collaborations
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Journal>()
            .HasMany(j => j.Collaborators)
            .WithOne()
            .HasForeignKey(c => c.JournalId)
            .OnDelete(DeleteBehavior.Cascade);

        ConfigureProblems(modelBuilder);
        ConfigureProblemMessages(modelBuilder);
        ConfigureNotifications(modelBuilder);
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

    private static void ConfigureProblems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Problem>(cfg =>
        {
            cfg.ToTable("Problems");
            cfg.HasKey(p => p.Id);

            cfg.Property(p => p.TourId).IsRequired();
            cfg.Property(p => p.CreatorId).IsRequired();
            cfg.Property(p => p.AuthorId).IsRequired();
            cfg.Property(p => p.Priority).IsRequired();
            cfg.Property(p => p.Description).IsRequired().HasMaxLength(2000);
            cfg.Property(p => p.CreationTime).IsRequired();
            cfg.Property(p => p.Category).IsRequired();
            cfg.Property(p => p.Status).IsRequired();
            cfg.Property(p => p.ResolvedAt);
            cfg.Property(p => p.AdminDeadline);
            cfg.Property(p => p.TouristComment).HasMaxLength(1000);

            cfg.HasIndex(p => p.TourId);
            cfg.HasIndex(p => p.CreatorId);
            cfg.HasIndex(p => p.AuthorId);
            cfg.HasIndex(p => p.Status);
            cfg.HasIndex(p => p.CreationTime);
        });
    }

    private static void ConfigureProblemMessages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProblemMessage>(cfg =>
        {
            cfg.ToTable("ProblemMessages");
            cfg.HasKey(pm => pm.Id);

            cfg.Property(pm => pm.ProblemId).IsRequired();
            cfg.Property(pm => pm.AuthorId).IsRequired();
            cfg.Property(pm => pm.Content).IsRequired().HasMaxLength(2000);
            cfg.Property(pm => pm.CreatedAt).IsRequired();

            cfg.HasIndex(pm => pm.ProblemId);
            cfg.HasIndex(pm => pm.AuthorId);
            cfg.HasIndex(pm => pm.CreatedAt);
        });
    }

    private static void ConfigureNotifications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(cfg =>
        {
            cfg.ToTable("Notifications");
            cfg.HasKey(n => n.Id);

            cfg.Property(n => n.UserId).IsRequired();
            cfg.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            cfg.Property(n => n.Type).IsRequired();
            cfg.Property(n => n.LinkId);
            cfg.Property(n => n.Timestamp).IsRequired();
            cfg.Property(n => n.IsRead).IsRequired();

            cfg.HasIndex(n => n.UserId);
            cfg.HasIndex(n => n.IsRead);
            cfg.HasIndex(n => n.Timestamp);
        });
    }
}