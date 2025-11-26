using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }

    public DbSet<Rating> Ratings { get; set; }
    public DbSet<UserLocation> UserLocations { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        ConfigureStakeholder(modelBuilder);


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

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }
}