using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityApp.Models
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (user.Year < DateTime.Now.Year - 90 || user.Year > DateTime.Now.Year)
            {
                errors.Add(new IdentityError
                {
                    Description = $"The year of birth must be between {DateTime.Now.Year - 90} " +
                        $"and {DateTime.Now.Year}"
                });
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
