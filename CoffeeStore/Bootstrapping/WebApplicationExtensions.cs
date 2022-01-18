using CoffeeStore.Context;
using Microsoft.EntityFrameworkCore;

namespace CoffeeStore.Bootstrapping
{
    public static class WebApplicationExtensions
    {
        public static async Task ExecuteMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CoffeeDbContext>();
            if (context != null)
            {
                if ((await context.Database.GetPendingMigrationsAsync()).GetEnumerator().MoveNext())
                    await context.Database.MigrateAsync();

                await context.Database.EnsureCreatedAsync();
                await context.SaveChangesAsync();
            }
        }
    }
}
