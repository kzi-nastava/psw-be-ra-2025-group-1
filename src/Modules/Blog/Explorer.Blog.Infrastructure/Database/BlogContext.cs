using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using BlogEntity = Explorer.Blog.Core.Domain.Blog; // "Blog is a namespace but is used as a type" -> fix for this, not really a good one but whatever for now..

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<BlogEntity> Blogs {get; set;} // Blogs table in DB
    public DbSet<Comment> Comments {get; set; } // Comments table in DB
    public DbSet<Vote> Votes { get; set; }
    public DbSet<BlogCollaborator> BlogCollaborators { get; set; }
    public DbSet<Stakeholders.Core.Domain.User> Users { get; set; } // read-only map
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.Entity<Core.Domain.Blog>().Property(b => b.Images).HasColumnType("text[]");

        modelBuilder.Entity<Core.Domain.Blog>().Property(b => b.Videos).HasColumnType("text[]");

        modelBuilder.Entity<Core.Domain.Blog>().Property(b => b.Status).IsRequired();

        modelBuilder.Entity<Core.Domain.Blog>().Property(b => b.LastModifiedDate).IsRequired(false); // nullable bcs the blog doesn't have to be updated if it's not needed
    
        modelBuilder.Entity<Core.Domain.Blog>().HasMany(b => b.Comments).WithOne().HasForeignKey("BlogId").IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Core.Domain.Blog>().HasMany(b => b.Votes).WithOne().HasForeignKey("BlogId").IsRequired().OnDelete(DeleteBehavior.Cascade);

        // mapiraj User tabelu iz stakeholders šeme, ali da migrations NE diraju tu tabelu
        modelBuilder.Entity<Explorer.Stakeholders.Core.Domain.User>(cfg =>
        {
            cfg.ToTable("Users", "stakeholders", t => t.ExcludeFromMigrations());
            cfg.HasKey(u => u.Id);
            cfg.HasIndex(u => u.Username).IsUnique();
        });

        // BlogCollaborator tabela u blog semi
        modelBuilder.Entity<BlogCollaborator>(cfg =>
        {
            cfg.ToTable("BlogCollaborators"); 
            cfg.HasKey(x => new { x.BlogId, x.UserId });
            cfg.HasIndex(x => new { x.BlogId, x.UserId }).IsUnique();

            cfg.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        });

        // Blog -> Collaborators
        modelBuilder.Entity<Explorer.Blog.Core.Domain.Blog>()
            .HasMany(b => b.Collaborators)
            .WithOne()
            .HasForeignKey(c => c.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}