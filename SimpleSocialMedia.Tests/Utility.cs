using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Moq;

namespace SimpleSocialMedia.Tests
{
    internal static class Utility
    {
        internal static void MockUserIdentityName(Controller controller)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] 
                    { 
                        new Claim(ClaimTypes.Name, "admin") 
                    }, "mock")
                );

            controller.ControllerContext.HttpContext = new DefaultHttpContext() 
            { 
                User = user 
            };
        }

        internal static IList<T> ToIList<T>(List<T> list)
        {
            return list;
        }

        internal static DbSet<T> ToDbSet<T>(List<T> list) where T : class
        {
            IQueryable<T> queryable = list.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();

            dbSet.As<IQueryable<T>>().Setup(l => l.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(l => l.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(l => l.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(l => l.GetEnumerator()).Returns(queryable.GetEnumerator());
            dbSet.Setup(l => l.Add(It.IsAny<T>())).Callback<T>(s => list.Add(s));

            return dbSet.Object;
        }
    }
}
