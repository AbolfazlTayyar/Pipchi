using Microsoft.EntityFrameworkCore;
using Pipchi.Infrastructure.Data;

namespace Pipchi.Api
{
    public static class ServiceCollectionExtensions
    {
        public static async Task ApplyMigrationsIfPending(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Database.GetPendingMigrations().Any())
                await context.Database.MigrateAsync();
        }
    }
}
