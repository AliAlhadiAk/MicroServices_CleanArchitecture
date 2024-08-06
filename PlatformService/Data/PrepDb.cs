using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder app, bool isProd)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

				try
				{
					context.Database.Migrate();
					Console.WriteLine("--> Migrations applied successfully.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"--> Could not run migrations: {ex.Message}");
				}

				SeedData(context, isProd);
			}
		}

		private static void SeedData(AppDbContext context, bool isProd)
		{
			if (!context.Platforms.Any())
			{
				Console.WriteLine("--> Seeding Data...");

				context.Platforms.AddRange(
					new Platfrom { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
					new Platfrom { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
					new Platfrom { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
				);

				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have data");
			}
		}
	}
}
