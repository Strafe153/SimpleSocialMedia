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

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

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
                    admin.ProfilePicture = 
                        File.ReadAllBytes(defaultProfilePicPath);
                    fileStream.Read(admin.ProfilePicture, 0, 
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
    }
}
