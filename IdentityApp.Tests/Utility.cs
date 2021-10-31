using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using IdentityApp.Models;
using Moq;

namespace IdentityApp.Tests
{
    internal static class Utility
    {
        internal static IQueryable<User> GetTestUsers()
        {
            List<User> testUsers = new List<User>()
            {
                new User() { Id = "test_id1", Email = "admin@gmail.com", UserName = "admin" },
                new User() { Id = "test_id2", Email = "qwerty@ukr.net", UserName = "qwerty" },
                new User() { Id = "test_id3", Email = "andrew.fox@gmail.com", UserName = "fox_a15" }
            };

            return testUsers.AsQueryable();
        }

        internal static List<IdentityRole> GetTestRoles()
        {
            List<IdentityRole> testRoles = new List<IdentityRole>()
            {
                new IdentityRole() { Name = "admin" },
                new IdentityRole() { Name = "moderator" },
                new IdentityRole() { Name = "user" }
            };

            return testRoles;
        }

        internal static IQueryable<LikedPost> GetTestLikedPosts()
        {
            List<LikedPost> testLikedPosts = new List<LikedPost>()
            {
                new LikedPost() { PostLikedId = "test_id1" },
                new LikedPost() { PostLikedId = "test_id2" },
                new LikedPost() { PostLikedId = "test_id3" }
            };

            return testLikedPosts.AsQueryable();
        }

        internal static List<Following> GetTestFollowings()
        {
            List<Following> followings = new List<Following>()
            {
                new Following() 
                { 
                    FollowedUser = new User(), 
                    FollowedUserId = "test_user_id1",
                    Reader = new User(),
                    ReaderId = "test_reader_id1"
                },
                new Following()
                {
                    FollowedUser = new User(),
                    FollowedUserId = "test_user_id2",
                    Reader = new User(),
                    ReaderId = "test_reader_id2"
                }
            };

            return followings;
        }

        internal static IList<string> GetTestUserRoles()
        {
            return ToIList(new List<string>() { "user", "admin" });
        }

        internal static void MockUserIdentityName(Controller controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.Name, "admin") }, "mock"));

            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        }

        internal static IList<T> ToIList<T>(List<T> list)
        {
            return list;
        }

        internal static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }
    }
}
