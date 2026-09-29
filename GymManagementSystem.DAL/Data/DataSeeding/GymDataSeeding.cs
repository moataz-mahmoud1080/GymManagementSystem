using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.DbContext;
using GymManagementSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.DAL.Data.DataSeeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext ,ILogger logger, string seedFolderPath, CancellationToken ct =default)
        {
            try
            {
                if (!await dbContext.Plans.AnyAsync(ct))
                {
                    var plans =LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");
                    if (plans.Any())
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Plans Seed Succcesfuly With Count {plans.Count}");
                    }

                    if (dbContext.ChangeTracker.HasChanges())
                        await dbContext.SaveChangesAsync();
                    else
                        logger.LogInformation("Plan Already seed ");
                }
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Gym Data Filed");
                throw;
            }



        }

        private static List<T> LoadDataFromJsonFile<T>(string folderPath , string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed Data File Was Not Found: {filePath} ");

            // Read Data From Jason File As JsonString (string)
            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            // Convert JsonString C# Object List<Plan>
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
            // Add To Database

        }
    }
}
