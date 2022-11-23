using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
	/// Contains extension for UserManager and RoleManager.
	/// </summary>
	public static class ManagersExtension
	{
		public async static Task<IdentityResult> AddUserToRole<TUser>(this UserManager<TUser> userManager, string UserName, string Role) where TUser : class
		{
			return await userManager.AddToRoleAsync(await userManager.FindByNameAsync(UserName), Role);
		}

		public async static Task<IdentityResult> AddUserToRoles<TUser>(this UserManager<TUser> userManager, string UserName, IQueryable<Role> Roles) where TUser : class
		{
			return await userManager.AddToRolesAsync(await userManager.FindByNameAsync(UserName), (await Roles.ToListAsync()).Select(e => e.Name));
		}
	}
	public static class Initializer
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

		private static readonly Modules Initialization = Modules.Database;

		public static async Task InitializeIdentity(IServiceProvider services)
		{
			if (Initialization != Modules.Identity)
				return;

			using (var context = new ZooISContext(services.GetRequiredService<DbContextOptions<ZooISContext>>()))
			{
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
			}

			var userManager = services.GetRequiredService<UserManager<User>>();
			var roleManager = services.GetRequiredService<RoleManager<Role>>();
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
					await roleManager.AddClaimAsync(await roleManager.FindByNameAsync("Admin"), new("AccessToUsers", "true"))
			};
			if (Results.Any(task => !task.Succeeded))
			{
				var E = new Exception("Something gone wrong");
				E.Data["Errors"] = Results.Where(task => !task.Succeeded).SelectMany(task => task.Errors);
				throw E;
			}
			Results = new() {
					await userManager.AddUserToRole("i-ivanov", "Caretaker"),
					await userManager.AddUserToRole("e-evanov", "Doctor"),
					await userManager.AddUserToRole("a-avanov", "Zootechnician"),
					await userManager.AddUserToRole("admin", "Admin"),
					await userManager.AddUserToRoles("test", roleManager.Roles.Where(e => e.Name != "Backend")) };
			if (Results.Any(task => !task.Succeeded))
			{
				var E = new Exception("Something gone wrong");
				E.Data["Errors"] = Results.Where(task => !task.Succeeded).SelectMany(task => task.Errors);
				throw E;
			}

			using (var context = new ZooISContext(services.GetRequiredService<DbContextOptions<ZooISContext>>()))
			{
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
		}

		public static async Task InitTaxons(ZooISContext context)
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
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Life).First(e => e.ScientificName == "Bios"),
					VernacularName = "Эвкариоты"
				});
			//Kingdoms
			context.Taxons.AddRange(
				new Taxon("Animalia")
				{
					Rank = TaxonRank.Kingdom,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Domain).First(e => e.ScientificName == "Eykaryota"),
					VernacularName = "Животные"
				});
			//Phylums
			context.Taxons.AddRange(
				new Taxon("Chordata")
				{
					Rank = TaxonRank.Phylum,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Kingdom).First(e => e.ScientificName == "Animalia"),
					VernacularName = "Хордовые"
				});
			//Classes
			context.Taxons.AddRange(
				new Taxon("Mammalia")
				{
					Rank = TaxonRank.Class,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Phylum).First(e => e.ScientificName == "Chordata"),
					VernacularName = "Млекопитающие"
				},
				new Taxon("Aves")
				{
					Rank = TaxonRank.Class,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Phylum).First(e => e.ScientificName == "Chordata"),
					VernacularName = "Птицы"
				});
			//Order
			context.Taxons.AddRange(
				new Taxon("Carnivora")
				{
					Rank = TaxonRank.Order,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Class).First(e => e.ScientificName == "Mammalia"),
					VernacularName = "Хищные"
				},
				new Taxon("Proboscidea")
				{
					Rank = TaxonRank.Order,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Class).First(e => e.ScientificName == "Mammalia"),
					VernacularName = "Хоботные"
				},
				new Taxon("Psittaciformes")
				{
					Rank = TaxonRank.Order,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Class).First(e => e.ScientificName == "Aves"),
					VernacularName = "Попугаеобразные"
				});
			//Families
			context.Taxons.AddRange(
				new Taxon("Felidae")
				{
					Rank = TaxonRank.Family,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Order).First(e => e.ScientificName == "Carnivora"),
					VernacularName = "Кошачьи"
				},
				new Taxon("Elephantidae")
				{
					Rank = TaxonRank.Family,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Order).First(e => e.ScientificName == "Proboscidea"),
					VernacularName = "Слоны"
				},
				new Taxon("Psittaculidae")
				{
					Rank = TaxonRank.Family,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Order).First(e => e.ScientificName == "Psittaciformes"),
					VernacularName = "Попугаи"
				});
			//Genus
			context.Taxons.AddRange(
				new Taxon("Panthera")
				{
					Rank = TaxonRank.Genus,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Family).First(e => e.ScientificName == "Felidae"),
					VernacularName = "Пантеры"
				},
				new Taxon("Loxodonta")
				{
					Rank = TaxonRank.Genus,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Family).First(e => e.ScientificName == "Elephantidae"),
					VernacularName = "Африканские слоны"
				},
				new Taxon("Melopsittacus")
				{
					Rank = TaxonRank.Genus,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Family).First(e => e.ScientificName == "Psittaculidae"),
					VernacularName = "Певчие попугаи"
				});
			//Species
			context.Taxons.AddRange(
				new Taxon("Panthera leo")
				{
					Rank = TaxonRank.Species,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Genus).First(e => e.ScientificName == "Panthera"),
					VernacularName = "Лев"
				},
				new Taxon("Loxodonta africana")
				{
					Rank = TaxonRank.Species,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Genus).First(e => e.ScientificName == "Loxodonta"),
					VernacularName = "Саванный слон"
				},
				new Taxon("Melopsittacus undulatus")
				{
					Rank = TaxonRank.Species,
					Parent = context.Taxons.Local.Where(e => (TaxonRank)e.RankId == TaxonRank.Genus).First(e => e.ScientificName == "Melopsittacus"),
					VernacularName = "Волнистый попугай"
				});
			await context.SaveChangesAsync();
		}

		public static async Task InitializeDatabase(IServiceProvider services)
		{
			if (Initialization != Modules.Database)
				return;
			var context = new ZooISContext(services.GetRequiredService<DbContextOptions<ZooISContext>>());
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
					Level = AlertLevel.Regular,
					Type = AlertType.Info,
					Message = "Тестовое сообщение",
					User = User
				},
				new Alert
				{
					Level = AlertLevel.Warning,
					Type = AlertType.Success,
					Message = "Тестовое сообщение",
					User = User
				},
				new Alert
				{
					Level = AlertLevel.Alert,
					Type = AlertType.Fail,
					Message = "Тестовое сообщение",
					User = User
				});
			await context.SaveChangesAsync();
		}

		public static async Task Initialize(IServiceProvider services)
		{
			switch (Initialization)
			{
				case Modules.Database:
					await InitializeDatabase(services);
					break;
				case Modules.Identity:
					await InitializeIdentity(services);
					break;
			}
		}
	}
}
