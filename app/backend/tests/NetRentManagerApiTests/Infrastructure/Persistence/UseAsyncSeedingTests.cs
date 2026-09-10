using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class UseAsyncSeedingTests
{
    [Fact]
    public async Task MigrateAsync_Triggers_UseAsyncSeeding_Even_Without_Pending_Migrations()
    {
        var calls = 0;
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseInMemoryDatabase($"use-async-seeding-{Guid.NewGuid():N}")
                .UseSeeding((_, _) => { })
                .UseAsyncSeeding((_, _, _) =>
                {
                    calls++;
                    return Task.CompletedTask;
                }));

        await using var app = builder.Build();

        await app.MigrateAsync();
        await app.MigrateAsync();

        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task MigrateAsync_Propagates_CancellationToken_To_UseAsyncSeeding()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseInMemoryDatabase($"use-async-seeding-cancel-{Guid.NewGuid():N}")
                .UseSeeding((_, _) => { })
                .UseAsyncSeeding((_, _, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return Task.CompletedTask;
                }));

        await using var app = builder.Build();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => app.MigrateAsync(cts.Token));
    }
}
