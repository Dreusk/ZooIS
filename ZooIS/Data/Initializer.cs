using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using ZooIS.Models;
using Microsoft.AspNetCore.Session;
using System.ComponentModel.DataAnnotations;

namespace ZooIS.Data
{
    /// <summary>
    /// Contains extension used for Identity.
    /// </summary>
    public static class ManagersExtension
    {
        public async static Task<IdentityResult> AddUserToRole<TUser>(this UserManager<TUser> userManager, ZooISContext context, string UserName, Role Role) where TUser : class
        {
            (await context.Users.FirstAsync(e => e.UserName == UserName)).Roles.Add(Role);
            await context.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async static Task<IdentityResult> AddUserToRole<TUser>(this UserManager<TUser> userManager, ZooISContext context, string UserName, string Role) where TUser : class
        {
            (await context.Users.FirstAsync(e => e.UserName == UserName)).Roles.Add(await context.Roles.FirstAsync(e => e.Name == Role));
            await context.SaveChangesAsync();
			return IdentityResult.Success;
        }

        public async static Task<IdentityResult> AddUserToRoles<TUser>(this UserManager<TUser> userManager, ZooISContext context, string UserName, IEnumerable<Role> Roles) where TUser : class
        {
			User user = await context.Users.FirstAsync(e => e.UserName == UserName);
			user.Roles = Roles.ToHashSet();
			await context.SaveChangesAsync();
            return IdentityResult.Success;
        }
    }

	public interface IInitializer
	{
		public Task Initialize();

    }

    public class Initializer: IInitializer
	{
        private enum Modules
        {
            [Display(Name = "Авторизациия")]
            Identity,
            [Display(Name = "База данных")]
            Database,
            [Display(Name = "Нет")]
            None
        };

        private readonly IConfiguration configuration;
		private readonly ZooISContext context;
		private readonly UserManager<User> userManager;
		private readonly RoleManager<Role> roleManager;

        private readonly Modules Initialization;

        public Initializer(IConfiguration configuration, ZooISContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
		{
			this.configuration = configuration;
			this.context = context;
			this.userManager = userManager;
			this.roleManager = roleManager;

			if (!Enum.TryParse<Modules>(configuration.GetSection("Initializer")["Module"], true, out Initialization))
				Initialization = Modules.None;
		}

		private async Task InitializeIdentity()
		{
			if (Initialization != Modules.Identity)
				return;

			//Clear identities
			context.Logs.RemoveRange(context.Logs);
			context.Alerts.RemoveRange(context.Alerts);
			context.UserClaims.RemoveRange(context.UserClaims);
			context.UserTokens.RemoveRange(context.UserTokens);
			context.UserLogins.RemoveRange(context.UserLogins);
			context.UserRoles.RemoveRange(context.UserRoles);
			context.RoleClaims.RemoveRange(context.RoleClaims);
			context.Employees.RemoveRange(context.Employees);
			context.Users.RemoveRange(context.Users);
			context.Roles.RemoveRange(context.Roles);
			await context.SaveChangesAsync();

			var Results = new HashSet<IdentityResult>();
			Results.UnionWith(new HashSet<IdentityResult>() {
				await userManager.CreateAsync(new User
				{
					UserName = "admin",
				}, "admin"),
				await userManager.CreateAsync(new User
				{
					UserName = "test",
				}, "test"),
				await userManager.CreateAsync(new User
				{
					UserName = "i-ivanov",
					Email = "i-ivanov@zo.org",
				}, "aaa111aaa111"),
				await userManager.CreateAsync(new User
				{
					UserName = "e-evanov",
					Email = "e-evanov@zo.org",
				}, "aaa111aaa111"),
				await userManager.CreateAsync(new User
				{
					UserName = "a-avanov",
					Email = "a-avanov@zo.org",
				}, "aaa111aaa111")});
			Results.UnionWith(new HashSet<IdentityResult> {
				await roleManager.CreateAsync(new Role
				{
					Name = "Admin",
					Display = "Сис. Администратор"
				}),
				await roleManager.CreateAsync(new Role
				{
					Name = "Caretaker",
					Display = "Смотритель"
				}),
				await roleManager.CreateAsync(new Role
				{
					Name = "Doctor",
					Display = "Доктор"
				}),
				await roleManager.CreateAsync(new Role
				{
					Name = "Zootechnician",
					Display = "Зоотехник"
				}),
				await roleManager.CreateAsync(new Role
				{
					Name = "Debug",
					Display = "Дебаг"
				})});
			if (Results.Any(task => !task.Succeeded))
			{
				var E = new Exception("Something gone wrong");
				E.Data["Errors"] = Results.Where(task => !task.Succeeded).SelectMany(task => task.Errors);
				throw E;
			}
			Results = new() {
					await roleManager.AddClaimAsync(await roleManager.FindByNameAsync("Debug"), new("AccessToRoles", "true")),
					await roleManager.AddClaimAsync(await roleManager.FindByNameAsync("Debug"), new("isDebug", "true")),

					await roleManager.AddClaimAsync(await roleManager.FindByNameAsync("Admin"), new("AccessToRoles", "true")),
					await roleManager.AddClaimAsync(await roleManager.FindByNameAsync("Admin"), new("AccessToUsers", "true")),
					await roleManager.AddClaimAsync(await roleManager.FindByNameAsync("Admin"), new("AccessToBackend", "true"))
            };
			if (Results.Any(task => !task.Succeeded))
			{
				var E = new Exception("Something gone wrong");
				E.Data["Errors"] = Results.Where(task => !task.Succeeded).SelectMany(task => task.Errors);
				throw E;
			}
			Results = new() {
					await userManager.AddUserToRole(context, "i-ivanov", "Caretaker"),
					await userManager.AddUserToRole(context, "e-evanov", "Doctor"),
					await userManager.AddUserToRole(context, "a-avanov", "Zootechnician"),
					await userManager.AddUserToRole(context, "admin", "Admin"),
					await userManager.AddUserToRoles(context, "test", context.Roles.Where(e => e.Name != "Backend")) };
			if (Results.Any(task => !task.Succeeded))
			{
				var E = new Exception("Something gone wrong");
				E.Data["Errors"] = Results.Where(task => !task.Succeeded).SelectMany(task => task.Errors);
				throw E;
			}

			context.Employees.AddRange(
				new Employee
				{
					UserId = (await userManager.FindByNameAsync("i-ivanov")).Id,
					Name = new()
					{
						GivenName = "Иван",
						FamilyName = "Иванов"
					}
				},
				new Employee
				{
					UserId = (await userManager.FindByNameAsync("e-evanov")).Id,
					Name = new()
					{
						GivenName = "Еван",
						FamilyName = "Еванов"
					}
				},
				new Employee
				{
					UserId = (await userManager.FindByNameAsync("a-avanov")).Id,
					Name = new()
					{
						GivenName = "Аван",
						FamilyName = "Аванов"
					}
				});
			await context.SaveChangesAsync();
		}

		private async Task InitTaxons(ZooISContext context)
		{
			//Life
			context.Taxons.AddRange(
				new Taxon("Bios")
				{
					Rank = TaxonRank.Life,
					VernacularName = "Жизнь"
				});
			//Domains
			context.Taxons.AddRange(
				new Taxon("Eykaryota")
				{
					Rank = TaxonRank.Domain,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Life).First(e => e.ScientificName == "Bios"),
					VernacularName = "Эвкариоты"
				});
			//Kingdoms
			context.Taxons.AddRange(
				new Taxon("Animalia")
				{
					Rank = TaxonRank.Kingdom,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Domain).First(e => e.ScientificName == "Eykaryota"),
					VernacularName = "Животные"
				});
			//Phylums
			context.Taxons.AddRange(
				new Taxon("Chordata")
				{
					Rank = TaxonRank.Phylum,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Kingdom).First(e => e.ScientificName == "Animalia"),
					VernacularName = "Хордовые"
				});
			//Classes
			context.Taxons.AddRange(
				new Taxon("Mammalia")
				{
					Rank = TaxonRank.Class,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Phylum).First(e => e.ScientificName == "Chordata"),
					VernacularName = "Млекопитающие"
				},
				new Taxon("Aves")
				{
					Rank = TaxonRank.Class,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Phylum).First(e => e.ScientificName == "Chordata"),
					VernacularName = "Птицы"
				});
			//Order
			context.Taxons.AddRange(
				new Taxon("Carnivora")
				{
					Rank = TaxonRank.Order,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Class).First(e => e.ScientificName == "Mammalia"),
					VernacularName = "Хищные"
				},
				new Taxon("Proboscidea")
				{
					Rank = TaxonRank.Order,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Class).First(e => e.ScientificName == "Mammalia"),
					VernacularName = "Хоботные"
				},
				new Taxon("Psittaciformes")
				{
					Rank = TaxonRank.Order,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Class).First(e => e.ScientificName == "Aves"),
					VernacularName = "Попугаеобразные"
				});
			//Families
			context.Taxons.AddRange(
				new Taxon("Felidae")
				{
					Rank = TaxonRank.Family,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Order).First(e => e.ScientificName == "Carnivora"),
					VernacularName = "Кошачьи"
				},
				new Taxon("Elephantidae")
				{
					Rank = TaxonRank.Family,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Order).First(e => e.ScientificName == "Proboscidea"),
					VernacularName = "Слоны"
				},
				new Taxon("Psittaculidae")
				{
					Rank = TaxonRank.Family,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Order).First(e => e.ScientificName == "Psittaciformes"),
					VernacularName = "Попугаи"
				});
			//Genus
			context.Taxons.AddRange(
				new Taxon("Panthera")
				{
					Rank = TaxonRank.Genus,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Family).First(e => e.ScientificName == "Felidae"),
					VernacularName = "Пантеры"
				},
				new Taxon("Loxodonta")
				{
					Rank = TaxonRank.Genus,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Family).First(e => e.ScientificName == "Elephantidae"),
					VernacularName = "Африканские слоны"
				},
				new Taxon("Melopsittacus")
				{
					Rank = TaxonRank.Genus,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Family).First(e => e.ScientificName == "Psittaculidae"),
					VernacularName = "Певчие попугаи"
				});
			//Species
			context.Taxons.AddRange(
				new Taxon("Panthera leo")
				{
					Rank = TaxonRank.Species,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Genus).First(e => e.ScientificName == "Panthera"),
					VernacularName = "Лев"
				},
				new Taxon("Loxodonta africana")
				{
					Rank = TaxonRank.Species,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Genus).First(e => e.ScientificName == "Loxodonta"),
					VernacularName = "Саванный слон"
				},
				new Taxon("Melopsittacus undulatus")
				{
					Rank = TaxonRank.Species,
					Parent = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Genus).First(e => e.ScientificName == "Melopsittacus"),
					VernacularName = "Волнистый попугай"
				});
			await context.SaveChangesAsync();
		}

		private async Task InitializeDatabase()
		{
			if (Initialization != Modules.Database)
				return;
			//Clear entities
			context.Alerts.RemoveRange(context.Alerts);
			(await context.Animals.Include(e => e.Children).ToListAsync()).ForEach(e =>
				e.Children.Clear());
			context.Animals.RemoveRange(context.Animals);
			context.Taxons.RemoveRange(context.Taxons);
			await context.SaveChangesAsync();
			//Add entities
			await InitTaxons(context);
			context.Animals.AddRange(
				new()
				{
					Name = "Пикки",
					Species = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Species).First(e => e.VernacularName == "Волнистый попугай"),
					Sex = Sex.Male,
					Status = new(),
					BirthDate = DateTime.Parse("11-11-2012"),
					PicturePath = "/local/Пикки.jpg"
				},
				new()
				{
					Name = "Сельдирион",
					Species = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Species).First(e => e.VernacularName == "Лев"),
					Sex = Sex.Male,
					Status = new(),
					BirthDate = DateTime.Parse("11-11-2013"),
					PicturePath = "/local/Сельдирион.jpg"
				},
				new()
				{
					Name = "Арфика",
					Species = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Species).First(e => e.VernacularName == "Лев"),
					Sex = Sex.Female,
					Status = new(),
					BirthDate = DateTime.Parse("11-11-2012"),
					PicturePath = "/local/Арфика.jpg"
				},
				new()
				{
					Name = "Бартоломей",
					Species = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Species).First(e => e.VernacularName == "Лев"),
					Sex = Sex.Male,
					Status = new(),
					BirthDate = DateTime.Parse("11-11-2017"),
					PicturePath = "/local/Бартоломей.jpg"
				},
				new()
				{
					Name = "Соловей",
					Species = context.Taxons.Local.Where(e => e.Rank == TaxonRank.Species).First(e => e.VernacularName == "Саванный слон"),
					Sex = Sex.Male,
					Status = new(),
					BirthDate = DateTime.Parse("11-11-2021"),
					PicturePath = "/local/Соловей.jpg"
				});
			context.Animals.Local.First(e => e.Name == "Бартоломей").Parents
				.UnionWith(new HashSet<Animal> {
					context.Animals.Local.First(e => e.Name == "Сельдирион"),
					context.Animals.Local.First(e => e.Name == "Арфика")
				});
			User User = await context.Users.FirstAsync(e => e.UserName == "test");
			context.Alerts.AddRange(
				new Alert
				{
					Url = "",
					Level = AlertLevel.Regular,
					Type = AlertType.Info,
					Message = "Тестовое сообщение",
					User = User
				},
				new Alert
				{
                    Url = "",
                    Level = AlertLevel.Warning,
					Type = AlertType.Success,
					Message = "Тестовое сообщение",
					User = User
				},
				new Alert
				{
                    Url = "",
                    Level = AlertLevel.Alert,
					Type = AlertType.Fail,
					Message = "Тестовое сообщение",
					User = User
				});
			await context.SaveChangesAsync();
		}

		public async Task Initialize()
		{
			switch (Initialization)
			{
				case Modules.Database:
					await InitializeDatabase();
					break;
				case Modules.Identity:
					await InitializeIdentity();
					break;
			}
		}
	}
}
