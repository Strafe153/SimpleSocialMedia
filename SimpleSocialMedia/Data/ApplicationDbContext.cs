using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostPicture> PostPictures { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<LikedPost> LikedPosts { get; set; }
        public DbSet<LikedComment> LikedComments { get; set; }
        public DbSet<CommentPicture> CommentPictures { get; set; }
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
                .WithOne(pic => pic.Post)
                .IsRequired();

            modelBuilder.Entity<PostPicture>()
                .HasOne(pic => pic.Post)
                .WithMany(p => p.PostPictures)
                .HasForeignKey(pic => pic.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.PostComments)
                .WithOne(c => c.Post)
                .IsRequired();

            modelBuilder.Entity<PostComment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.PostComments)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedPost>()
                .HasKey(lp => new 
                { 
                    lp.UserWhoLikedId, 
                    lp.PostLikedId 
                });

            modelBuilder.Entity<LikedPost>()
                .HasOne(lp => lp.UserWhoLiked)
                .WithMany(u => u.LikedPosts)
                .HasForeignKey(lp => lp.UserWhoLikedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LikedPost>()
                .HasOne(lp => lp.PostLiked)
                .WithMany(p => p.LikedPosts)
                .HasForeignKey(lp => lp.PostLikedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostComment>()
                .HasMany(c => c.CommentPictures)
                .WithOne(p => p.Comment)
                .IsRequired();

            modelBuilder.Entity<CommentPicture>()
                .HasOne(p => p.Comment)
                .WithMany(c => c.CommentPictures)
                .HasForeignKey(p => p.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedComment>()
                .HasKey(lc => new
                {
                    lc.UserWhoLikedId,
                    lc.CommentLikedId
                });

            modelBuilder.Entity<LikedComment>()
                .HasOne(lc => lc.UserWhoLiked)
                .WithMany(u => u.LikedComments)
                .HasForeignKey(lc => lc.UserWhoLikedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LikedComment>()
                .HasOne(lc => lc.CommentLiked)
                .WithMany(c => c.LikedComments)
                .HasForeignKey(lc => lc.CommentLikedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Following>()
                .HasKey(f => new
                {
                    f.FollowedUserId,
                    f.ReaderId
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