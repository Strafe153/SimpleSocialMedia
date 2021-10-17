using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.Tests
{
    internal static class Utility
    {
        internal static IQueryable<User> GetTestUsers()
        {
            var users = new List<User>();

            users.AddRange(new List<User>()
            {
                new User()
                {
                    Id = "c8fe8b58-e3b6-42bd-8e2e-ef4863b91628",
                    Email = "admin@gmail.com",
                    UserName = "admin"
                },
                new User()
                {
                    Id = "f6e8a62f-7153-4389-a5b5-51297bc4a133",
                    Email = "qwerty@ukr.net",
                    UserName = "qwerty"
                },
                new User()
                {
                    Id = "6457ed17-dfff-44b1-9c93-edd26edd2624",
                    Email = "andrew.fox@gmail.com",
                    UserName = "fox_a15"
                }
            });

            return users.AsQueryable();
        }

        internal static IQueryable<LikedPost> GetTestLikedPosts()
        {
            List<LikedPost> testPosts = new List<LikedPost>();

            testPosts.AddRange(new List<LikedPost>
            {
                new LikedPost()
                {
                    PostId = "test_id"
                },
                new LikedPost()
                {
                    PostId = "qwerty"
                }
            });

            return testPosts.AsQueryable();
        }

        internal static void MockUserIdentityName(Controller controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "admin")
            }, "mock"));

            controller.ControllerContext.HttpContext = new DefaultHttpContext()
                { User = user };
        }

        internal static IList<T> ToIList<T>(List<T> list)
        {
            return list;
        }
    }
}
