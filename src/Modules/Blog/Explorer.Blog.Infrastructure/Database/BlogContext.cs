using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using BlogEntity = Explorer.Blog.Core.Domain.Blog; // "Blog is a namespace but is used as a type" -> fix for this, not really a good one but whatever for now..

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<BlogEntity> Blogs {get; set;} // Blogs table in DB
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.Entity<Core.Domain.Blog>().Property(b => b.Images).HasColumnType("text[]");

        modelBuilder.Entity<Core.Domain.Blog>().Property(b => b.Status).IsRequired();

        modelBuilder.Entity<Core.Domain.Blog>().Property(b => b.LastModifiedDate).IsRequired(false); // nullable bcs the blog doesn't have to be updated if it's not needed
    }
}