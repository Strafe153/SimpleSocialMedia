using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Models;

namespace IdentityApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;

                try
                {
                    UserManager<User> userManager = services.GetRequiredService<UserManager<User>>();
                    RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    IWebHostEnvironment appEnvironment = services.GetRequiredService<IWebHostEnvironment>();

                    await RoleInitializer.InitializeAsync(userManager, roleManager, appEnvironment);
                }
                catch (Exception ex)
                {
                    ILogger logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured while seeding a database");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
