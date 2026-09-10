using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();

    public DbSet<PropertyStatusCatalogEntry> PropertyStatuses => Set<PropertyStatusCatalogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}