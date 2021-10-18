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
            List<User> testUsers = new List<User>();

            testUsers.AddRange(new List<User>()
            {
                new User()
                {
                    Id = "zxv34r",
                    Email = "admin@gmail.com",
                    UserName = "admin"
                },
                new User()
                {
                    Id = "45hydgfs",
                    Email = "qwerty@ukr.net",
                    UserName = "qwerty"
                },
                new User()
                {
                    Id = "refv3",
                    Email = "andrew.fox@gmail.com",
                    UserName = "fox_a15"
                }
            });

            return testUsers.AsQueryable();
        }

        internal static List<IdentityRole> GetTestRoles()
        {
            List<IdentityRole> testRoles = new List<IdentityRole>();

            testRoles.AddRange(new List<IdentityRole>()
            {
                new IdentityRole() { Name = "admin" },
                new IdentityRole() { Name = "moderator" },
                new IdentityRole() { Name = "user" }
            });

            return testRoles;
        }

        internal static IList<string> GetTestUserRoles()
        {
            return ToIList(new List<string>() { "user", "admin" });
        }

        internal static IQueryable<LikedPost> GetTestLikedPosts()
        {
            List<LikedPost> testLikedPosts = new List<LikedPost>();

            testLikedPosts.AddRange(new List<LikedPost>
            {
                new LikedPost() { PostId = "test_id" },
                new LikedPost() { PostId = "qwerty" }
            });

            return testLikedPosts.AsQueryable();
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
