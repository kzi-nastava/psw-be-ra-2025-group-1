using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Shopping;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Tour> Tour { get; set; }
    public DbSet<Facility> Facility { get; set; }
    public DbSet<MeetUp> MeetUp { get; set; }
    public DbSet<PersonEquipment> PersonEquipment { get; set; }
    public DbSet<ProblemMessage> ProblemMessages { get; set; }
    public DbSet<TransportTime> TransportTime { get; set; }
    public DbSet<TourExecution> TourExecutions { get; set; }
    public DbSet<Keypoint> Keypoints { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");
        
        ConfigureProblemMessages(modelBuilder);
    }

    private static void ConfigureProblemMessages(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<ProblemMessage>()
            .HasKey(pm => pm.Id);

        modelBuilder.Entity<ProblemMessage>()
            .Property(pm => pm.Content)
            .IsRequired()
            .HasMaxLength(2000);

        modelBuilder.Entity<ProblemMessage>()
            .Property(pm => pm.AuthorId)
            .IsRequired();

        modelBuilder.Entity<ProblemMessage>()
            .Property(pm => pm.CreatedAt)
            .IsRequired();

        modelBuilder.Entity<ProblemMessage>()
            .Property(pm => pm.ProblemId)
            .IsRequired();

        
        modelBuilder.Entity<ProblemMessage>()
            .HasIndex(pm => pm.ProblemId);
        
        modelBuilder.Entity<ProblemMessage>()
            .HasIndex(pm => pm.AuthorId);

        // One-Many relationship between Tour and Keypoint
        modelBuilder.Entity<Tour>()
            .HasMany(e => e.Keypoints)
            .WithOne()
            .HasForeignKey("TourId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tour>()
            .HasMany(t => t.Equipment)
            .WithMany()
            .UsingEntity(j => j.ToTable("TourEquipment"));
    }
}