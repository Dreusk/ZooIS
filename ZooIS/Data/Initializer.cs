using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using ZooIS.Models;

namespace ZooIS.Data
{
    public static class Initializer
    {
        private static bool isInitialize = true;
        private static bool isInitializeIdentity = false;

        public static async void InitializeIdentity(IServiceProvider services)
        {
            if (isInitializeIdentity)
            {
                using (var context = new ZooISContext(services.GetRequiredService<DbContextOptions<ZooISContext>>()))
                {
                    //Clear identities
                    context.Logs.RemoveRange(context.Logs);
                    context.UserClaims.RemoveRange(context.UserClaims);
                    context.UserTokens.RemoveRange(context.UserTokens);
                    context.UserLogins.RemoveRange(context.UserLogins);
                    context.UserRoles.RemoveRange(context.UserRoles);
                    context.RoleClaims.RemoveRange(context.RoleClaims);
                    context.Users.RemoveRange(context.Users);
                    context.Roles.RemoveRange(context.Roles);
                    await context.SaveChangesAsync();
                }
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var Results = new {
					Users = new List<IdentityResult>(),
					Roles = new List<IdentityResult>()
				};
                Results.Users.AddRange(
					await userManager.CreateAsync(new IdentityUser
					{
						UserName = "admin",
					}, "admin"),
					await userManager.CreateAsync(new IdentityUser
					{
						UserName = "test",
					}, "test"),
					await userManager.CreateAsync(new IdentityUser
					{
						UserName = "i-ivanov",
						Email    = "i-ivanov@zo.org",
					}, "aaa111aaa111"),
					await userManager.CreateAsync(new IdentityUser
					{
						UserName = "e-evanov",
						Email    = "e-evanov@zo.org",
					}, "aaa111aaa111"),
					await userManager.CreateAsync(new IdentityUser
					{
						UserName = "a-avanov",
						Email    = "a-avanov@zo.org",
					}, "aaa111aaa111"));
                Results.Roles.AddRange(
					await roleManager.CreateAsync(new IdentityRole
					{
						Name = "Admin"
					}),
					await roleManager.CreateAsync(new IdentityRole
					{
						Name = "Caretaker"
					}),
					await roleManager.CreateAsync(new IdentityRole
					{
						Name = "Doctor"
					}),
					await roleManager.CreateAsync(new IdentityRole
					{
						Name = "Zootechnician"}));
                if (Results.Users.Concat(Results.Roles).All(e => e.Succeed))
                {
                    await userManager.AddToRoleAsync(await userManager.FindByNameAsync("i-ivanov"), "Caretaker");
					await userManager.AddToRoleAsync(await userManager.FindByNameAsync("e-evanov"), "Doctor");
					await userManager.AddToRoleAsync(await userManager.FindByNameAsync("a-avanov"), "Zootechnician");
					await userManager.AddToRoleAsync(await userManager.FindByNameAsync("admin"), "Admin");
					var Dummy = await userManager.FindByNameAsync("test").Result();
					foreach (var Role in roleManager.Roles)
						await userManager.AddToRoleAsync(Dummy.Name, Role.Name);
                }
            }
        }

        public static async void Initialize(IServiceProvider services)
        {
            if (isInitialize)
            {
                var context = new ZooISContext(services.GetRequiredService<DbContextOptions<ZooISContext>>());
                //Clear entities
                context.Animal.RemoveRange(context.Animal);
                context.Species.RemoveRange(context.Species);
                await context.SaveChangesAsync();
                //Add entities
                context.Species.AddRange(
                    new Species("Panthera leo")
                    {
                        VernacularName = "Лев"
                    },
                    new Species("Elephantidae")
                    {
                        VernacularName = "Слон"
                    },
                    new Species("Psittacines")
                    {
                        VernacularName = "Попугай"
                    });
                context.Animal.AddRange(
                    new Animal("Пикки")
                    {
                        Age = 10,
                        Species = context.Species.Local.First(e => e.VernacularName == "Попугай"),
                        Sex = Sex.Male,
                        Status = new() {
                            Health = Health.Healthy,
                            Mood = Mood.Vivid }
                    },
                    new Animal("Бартоломей")
                    {
                        Age = 10,
                        Species = context.Species.Local.First(e => e.VernacularName == "Лев"),
                        Sex = Sex.Male,
                        Status = new()
                        {
                            Health = Health.Healthy,
                            Mood = Mood.Vivid
                        }
                    });
                await context.SaveChangesAsync();
            }
        }
    }
}
