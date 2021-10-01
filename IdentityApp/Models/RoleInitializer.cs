using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IdentityApp.Models
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, 
            IWebHostEnvironment appEnvironment)
        {
            const string ADMIN_NAME = "strafe";
            const string ADMIN_EMAIL = "strafe@gmail.com";
            const string ADMIN_PASSWORD = "qwer1234zxcv";

            string defaultProfilePicPath = 
                $"{appEnvironment.WebRootPath}/Files/default_profile_pic.jpg";

            await CheckRoleOnInitializing(roleManager, "user");
            await CheckRoleOnInitializing(roleManager, "admin");

            if (await userManager.FindByNameAsync(ADMIN_NAME) == null)
            {
                User admin = new User()
                {
                    UserName = ADMIN_NAME,
                    Email = ADMIN_EMAIL
                };

                using (FileStream fileStream = new FileStream(
                    defaultProfilePicPath, FileMode.Open, FileAccess.Read))
                {
                    admin.ProfilePicture = await File
                        .ReadAllBytesAsync(defaultProfilePicPath);
                    await fileStream.ReadAsync(admin.ProfilePicture, 0, 
                        Convert.ToInt32(fileStream.Length));
                }

                IdentityResult result = await userManager
                    .CreateAsync(admin, ADMIN_PASSWORD);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

        private static async Task CheckRoleOnInitializing(
            RoleManager<IdentityRole> roleManager, string role)
        {
            if (await roleManager.FindByNameAsync(role) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
