using GymSystem.DAL;
using GymSystem.DAL.AppDbContexts;
using Microsoft.EntityFrameworkCore;

namespace GymSystemG04.Extentions
{
    public static class ProgramExtention
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Migrations Applied To The database");
            }

            //C:\Users\LOQ\Source\Repos\GymSystem07G04\GymSystemG04\wwwroot\files\
            var folderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "files");
            await DataSeeder.SeedAsync(dbContext, logger, folderPath);

        }
    }
}
