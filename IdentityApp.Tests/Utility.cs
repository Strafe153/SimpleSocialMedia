using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;

namespace IdentityApp.Tests
{
    internal static class Utility
    {
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
