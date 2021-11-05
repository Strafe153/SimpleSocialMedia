using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Models;
using IdentityApp.Interfaces;
using IdentityApp.ControllerRepositories;

namespace IdentityApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserValidator<User>, CustomUserValidator>();
            services.AddScoped<IAccountControllable, AccountControllerRepository>();
            services.AddScoped<IHomeControllable, HomeControllerRepository>();
            services.AddScoped<IPostControllable, PostControllerRepository>();
            services.AddScoped<IPostPictureControllable, PostPictureRepository>();
            services.AddScoped<IRolesControllable, RolesControllerRepository>();
            services.AddScoped<IUsersControllable, UsersControllerRepository>();
            services.AddScoped<IPostCommentControllable, PostCommentRepository>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "1234567890abcdefghijklmnopqrstuvwxyz" +
                    @"ABCDEFGHIJKLMNOPQRSTUVWXYZ-_.?=+`~!#$;()[]{}*&^:%,\/<>@ ";
            }).AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
