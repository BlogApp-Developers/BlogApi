using BlogApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data;

public class BlogDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<UserTopic> UserTopics { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }

    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTopic>()
            .HasKey(ut => new { ut.UserId, ut.TopicId });

        modelBuilder.Entity<UserTopic>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.Topics)
            .HasForeignKey(ut => ut.UserId);

        modelBuilder.Entity<UserTopic>()
            .HasOne(ut => ut.Topic)
            .WithMany(t => t.Users)
            .HasForeignKey(ut => ut.TopicId);

        modelBuilder.Entity<Blog>()
            .HasOne(b => b.User)
            .WithMany(u => u.Blogs)
            .HasForeignKey(b => b.UserId);

        modelBuilder.Entity<Blog>()
            .HasOne(b => b.Topic)
            .WithMany(t => t.Blogs)
            .HasForeignKey(b => b.TopicId);

        modelBuilder.Entity<Comment>()
                    .HasOne<Blog>()  
                    .WithMany()     
                    .HasForeignKey(c => c.BlogId);

        modelBuilder.Entity<Comment>()
            .HasOne<User>()  
            .WithMany()     
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Blog)
            .WithMany(b => b.Likes)
            .HasForeignKey(l => l.BlogId);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId);
    }
}
