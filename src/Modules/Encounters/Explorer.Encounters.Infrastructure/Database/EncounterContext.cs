using Microsoft.EntityFrameworkCore;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Infrastructure.Database;

public class EncounterContext : DbContext
{
    public DbSet<Encounter> Encounters { get; set; }
    public DbSet<ActiveEncounter> ActiveEncounters { get; set; }
    public DbSet<CompletedEncounter> CompletedEncounters { get; set; }
    public DbSet<Requirement> Requirements { get; set; }
    public DbSet<TouristStats> TouristStats { get; set; }
    public EncounterContext(DbContextOptions<EncounterContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("encounters");

        modelBuilder.Entity<Encounter>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Encounter>()
            .Property(e => e.Type)
            .HasConversion<int>();

        modelBuilder.Entity<ActiveEncounter>()
            .HasMany(r => r.Requirements)
            .WithOne()
            .HasForeignKey("ActiveEncounterId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
