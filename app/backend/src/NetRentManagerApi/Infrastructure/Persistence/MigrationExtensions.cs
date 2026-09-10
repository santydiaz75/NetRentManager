using Microsoft.EntityFrameworkCore;

namespace NetRentManagerApi.Infrastructure.Persistence;

public static class MigrationExtensions
{
    public static async Task MigrateAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(app);

        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync(cancellationToken);
            return;
        }

        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}