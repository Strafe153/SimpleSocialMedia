using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostPicture> PostPictures { get; set; }
        public DbSet<LikedPost> LikedPosts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(user => user.Posts)
                .WithOne(post => post.User)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .HasOne(post => post.User)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(post => post.PostPictures)
                .WithOne(postPicture => postPicture.Post)
                .IsRequired();

            modelBuilder.Entity<PostPicture>()
                .HasOne(postPicture => postPicture.Post)
                .WithMany(post => post.PostPictures)
                .HasForeignKey(postPicture => postPicture.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedPost>()
                .HasKey(likedPost => new 
                { 
                    likedPost.UserId, 
                    likedPost.PostId 
                });

            modelBuilder.Entity<LikedPost>()
                .HasOne(likedPost => likedPost.User)
                .WithMany(user => user.LikedPosts)
                .HasForeignKey(likedPost => likedPost.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LikedPost>()
                .HasOne(likedPost => likedPost.Post)
                .WithMany(post => post.LikedPosts)
                .HasForeignKey(likedPost => likedPost.PostId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}