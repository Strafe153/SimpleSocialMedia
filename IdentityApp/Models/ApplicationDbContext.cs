using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostPicture> PostPictures { get; set; }
        public DbSet<LikedPost> LikedPosts { get; set; }
        public DbSet<Following> Followings { get; set; }

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
                    likedPost.UserWhoLikedId, 
                    likedPost.PostLikedId 
                });

            modelBuilder.Entity<LikedPost>()
                .HasOne(likedPost => likedPost.UserWhoLiked)
                .WithMany(user => user.LikedPosts)
                .HasForeignKey(likedPost => likedPost.UserWhoLikedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LikedPost>()
                .HasOne(likedPost => likedPost.PostLiked)
                .WithMany(post => post.LikedPosts)
                .HasForeignKey(likedPost => likedPost.PostLikedId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Following>()
                .HasKey(followedUser => new
                {
                    followedUser.FollowedUserId,
                    followedUser.ReaderId
                });

            modelBuilder.Entity<Following>()
                .HasOne(followedUser => followedUser.Reader)
                .WithMany(follower => follower.FollowingUsers)
                .HasForeignKey(followedUser => followedUser.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Following>()
                .HasOne(follower => follower.FollowedUser)
                .WithMany(followed => followed.Followers)
                .HasForeignKey(follower => follower.FollowedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}