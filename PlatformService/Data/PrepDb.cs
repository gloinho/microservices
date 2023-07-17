using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext dbContext, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    dbContext.Database.Migrate(); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Couldnt run migrations {ex.Message}");
                }
            }
            if (!dbContext.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                dbContext.Platforms.AddRange(
                        new Platform() { Name="Dot Net", Publisher = "Microsoft", Cost="Free"},
                        new Platform() { Name="SQL Server Express", Publisher = "Microsoft", Cost="Free"},
                        new Platform() { Name="Kubernetes", Publisher = "Microsoft", Cost="Free"}
                    );
                dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data.");
            }
        }
       
    }
}
