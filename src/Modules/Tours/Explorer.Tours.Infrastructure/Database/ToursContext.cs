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
    public DbSet<TransportTime> TransportTime { get; set; }
    public DbSet<TourExecution> TourExecutions { get; set; }
    public DbSet<Keypoint> Keypoints { get; set; }
    public DbSet<KeypointProgress> KeypointProgress { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        // One-Many relationship between Tour and Keypoint
        modelBuilder.Entity<Tour>()
            .HasMany(e => e.Keypoints)
            .WithOne()
            .HasForeignKey("TourId")
            .OnDelete(DeleteBehavior.Cascade);

        // One-Many relationship between TourExecution and KeypointProgress
        modelBuilder.Entity<TourExecution>()
        .HasMany(e => e.KeypointProgresses)
        .WithOne()
        .HasForeignKey("TourExecutionId")
        .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one progress per keypoint per execution
        modelBuilder.Entity<KeypointProgress>()
            .HasIndex("TourExecutionId", "KeypointId")
            .IsUnique();
    }
}