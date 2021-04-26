using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Entities;

namespace Web_Games_Store_Locus.Helpers
{
    public static class SeedDataExtention
    {
        public static IWebHost SeedDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                try
                {
                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return webHost;
        }
        public static void SeedData(IServiceProvider services,
 IWebHostEnvironment env,
 IConfiguration config)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var managerRole = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                SeedUsers(manager, managerRole,env, context);
            }
        }
        private static void SeedUsers(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env,ApplicationContext context)
        {
            var roleName = "Admin";
            if (roleManager.FindByNameAsync(roleName).Result == null)
            {
                var resultAdminRole = roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Admin"
                }).Result;
                var resultUserRole = roleManager.CreateAsync(new IdentityRole
                {
                    Name = "User"
                }).Result;
            }
            string email = "admin@gmail.com";
            var admin = new User
            {
                Email = email,
                UserName = email
            };    
            var resultAdmin = userManager.CreateAsync(admin, "Qwerty1-").Result;
            resultAdmin = userManager.AddToRoleAsync(admin, "Admin").Result;
            var adminInfo = new UserInfo()
            {
                Image = env.WebRootPath + "/Images/UserIcons/default-user-image.png",
                Address = "",
                Alias = "admin",
                Birth = DateTime.Today,
                IsBanned = false,
                Username = "admin@gmail.com",
                Id = admin.Id
            };
            var vlad = new User
            {
                Email = "vladdjuga@gmail.com",
                UserName = "vladdjuga@gmail.com"
            };
            var resultVlad = userManager.CreateAsync(vlad, "Qwerty1-").Result;
            resultVlad = userManager.AddToRoleAsync(vlad, "User").Result;
            var vladInfo = new UserInfo()
            {
                Image = env.WebRootPath + "/Images/UserIcons/default-user-image.png",
                Address = "",
                Alias = "vlad",
                Birth = DateTime.Today,
                IsBanned = false,
                Username = "vladdjuga@gmail.com",
                Id = vlad.Id
            };
            context.UserInfos.Add(adminInfo);
            context.UserInfos.Add(vladInfo);
            context.SaveChanges();
        }
    }
}
