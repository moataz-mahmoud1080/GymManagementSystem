using GymManagementSystem.DAL.Data.DbContext;
using GymManagementSystem.DAL.Data.DataSeeding; // 1. أضف الـ namespace ده
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GymManagementSystem.DAL.Data.Models;

namespace GymManagementSystem.PL.Extention
{
    public static class ProgramExtenton
    {
        public static async Task MigrateAndSeedAsync(this WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            var roleManager= scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager= scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                logger.LogInformation($"Applying {pendingMigrations.Count()} Pending Migrations ... ");

            await dbContext.Database.MigrateAsync();

            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");

            // 2. استدعاء ميثود الـ Seed التي كانت مفقودة
            await GymDataSeeding.SeedAsync(dbContext, logger, seedFolderPath);
            await IdentityDataSeading.SeedAsync(roleManager, userManager, logger);
        }
    }
}