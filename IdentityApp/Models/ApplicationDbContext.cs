using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostPicture> PostPictures { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.PostPictures)
                .WithOne(pp => pp.Post)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .HasMany(post => post.LikedPosts)
                .WithOne(likedPost => likedPost.Post)
                .HasForeignKey(likedPost => likedPost.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostPicture>()
                .HasOne(pp => pp.Post)
                .WithMany(p => p.PostPictures)
                .HasForeignKey(pp => pp.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedPost>()
                .HasKey(pl => new { pl.UserId, pl.PostId });

            modelBuilder.Entity<LikedPost>()
                .HasOne(pl => pl.User)
                .WithMany(u => u.LikedPosts)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LikedPost>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.LikedPosts)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}