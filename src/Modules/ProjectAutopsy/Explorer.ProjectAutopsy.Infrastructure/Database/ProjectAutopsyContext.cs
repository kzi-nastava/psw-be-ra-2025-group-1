using Microsoft.EntityFrameworkCore;
using Explorer.ProjectAutopsy.Core.Domain;

namespace Explorer.ProjectAutopsy.Infrastructure.Database;

public class ProjectAutopsyContext : DbContext
{
    public DbSet<AutopsyProject> AutopsyProjects { get; set; }
    public DbSet<RiskSnapshot> RiskSnapshots { get; set; }
    public DbSet<AIReport> AIReports { get; set; }

    public ProjectAutopsyContext(DbContextOptions<ProjectAutopsyContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("autopsy");

        // AutopsyProject configuration
        modelBuilder.Entity<AutopsyProject>(entity =>
        {
            entity.ToTable("autopsy_projects");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.GitHubRepo)
                .HasMaxLength(200);
            
            entity.HasIndex(e => e.Name)
                .IsUnique();
        });

        // RiskSnapshot configuration
        modelBuilder.Entity<RiskSnapshot>(entity =>
        {
            entity.ToTable("risk_snapshots");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Metrics)
                .HasColumnType("jsonb");
            
            entity.Property(e => e.Trend)
                .HasConversion<string>();
            
            entity.HasIndex(e => new { e.ProjectId, e.CreatedAt });
        });

        // AIReport configuration
        modelBuilder.Entity<AIReport>(entity =>
        {
            entity.ToTable("ai_reports");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Status)
                .HasConversion<string>();
            
            entity.Property(e => e.Content)
                .HasColumnType("jsonb");
            
            entity.Property(e => e.ModelUsed)
                .HasMaxLength(100);
            
            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(2000);
            
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.RiskSnapshotId);
        });
    }
}
