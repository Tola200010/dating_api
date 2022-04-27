using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    public class AppDbInitialer
    {
        public static async void SeedData(IApplicationBuilder applicationBuilder){
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            await context!.Database.MigrateAsync();
            await Seed.SeedUsers(userManager,roleManager);
        }
    }
}