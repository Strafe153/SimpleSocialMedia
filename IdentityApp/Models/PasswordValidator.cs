using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IdentityApp.Models
{
    public class PasswordValidator : IPasswordValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager,
            User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (user?.Year > DateTime.Now.Year || user?. Year < 1900)
            {
                errors.Add(new IdentityError()
                {
                    Description = "The year must be between 1900 and " + 
                        DateTime.Now.Year
                });
            }

            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success
                : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
