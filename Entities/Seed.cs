using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    public class Seed
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public Seed(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async static Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users!.AnyAsync()) return;
            var userData = System.IO.File.ReadAllText(@"D:\05 - Project\Dating App\API\Entities\UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            int userId = 1;
            var roles = new List<AppRole>{
                new AppRole{Name="Member"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"}
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
            foreach (var user in users!)
            {
                AppUser appUser = new AppUser();
                appUser.UserName = user.Name!.ToLower();
                appUser.KnownAs = user.KnownAs;
                appUser.Gender = user.Gender;
                appUser.Introduction = user.Introduction;
                appUser.LookingFor = user.LookingFor;
                appUser.Interests = user.Interests;
                appUser.City = user.City;
                appUser.DateOfBirth = user.DateOfBirth.ToUniversalTime();
                appUser.Country = user.Country;
                appUser.Name = user.Name!.ToLower();
                List<Photo> photos = new();
                foreach (var photo in user.Photos!)
                {
                    var getPhoto = new Photo();
                    getPhoto.Url = photo.Url;
                    getPhoto.AppUserId = userId;
                    getPhoto.IsMain = true;
                    getPhoto.PublicId = "0";
                    photos.Add(getPhoto);
                }
                appUser.Photos = photos; 
                await userManager.CreateAsync(appUser, "Pa$$w0rd");
                await userManager.AddToRoleAsync(appUser, "Member");
            }
            var admin = new AppUser
            {
                UserName = "admin",
                Name = "admin"
            };
            await userManager.CreateAsync(admin, "Pw12345");
            await userManager.AddToRolesAsync(admin, new[] { "admin", "Moderator" });
        }
        public static IEnumerable<AppUser> GetUser()
        {
            var userData = System.IO.File.ReadAllText(@"D:\05 - Project\Dating App\API\Entities\UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData!);
            List<AppUser> appUsers = new();
            int userId = 1;
            foreach (var user in users!)
            {
                AppUser appUser = new AppUser();
                appUser.Id = userId++;
                appUser.Id = userId;
                appUser.KnownAs = user.KnownAs;
                appUser.Gender = user.Gender;
                appUser.Introduction = user.Introduction;
                appUser.LookingFor = user.LookingFor;
                appUser.Interests = user.Interests;
                appUser.City = user.City;
                appUser.Country = user.Country;
                using var h = new HMACSHA512();
                appUser.Name = user.Name!.ToLower();
                //  appUser.PasswordHash = h.ComputeHash(Encoding.UTF8.GetBytes("12345"));
                //  appUser.PasswordSalt = h.Key;
                List<Photo> photos = new();
                foreach (var photo in user.Photos!)
                {
                    var getPhoto = new Photo();
                    getPhoto.Url = photo.Url;
                    getPhoto.AppUserId = userId;
                    getPhoto.IsMain = true;
                    getPhoto.PublicId = "0";
                    photos.Add(getPhoto);
                }
                appUser.Photos = photos;
                appUsers.Add(appUser);
            }
            return appUsers!;
        }
    }
}