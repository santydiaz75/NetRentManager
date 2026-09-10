using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class PropertyStatusConversionTests
{
    [Theory]
    [InlineData(PropertyStatus.Available)]
    [InlineData(PropertyStatus.Rented)]
    [InlineData(PropertyStatus.Maintenance)]
    public void PropertyStatus_Is_Stored_As_Text_And_RoundTrips(PropertyStatus status)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=netrentmanager_conversion_tests;Username=postgres;Password=postgres")
            .Options;

        using var context = new AppDbContext(options);

        var propertyType = context.Model.FindEntityType(typeof(Property));
        Assert.NotNull(propertyType);

        var statusProperty = propertyType.FindProperty(nameof(Property.Status));
        Assert.NotNull(statusProperty);

        Assert.Equal(typeof(PropertyStatus), statusProperty.ClrType);
        Assert.Equal("character varying(32)", statusProperty.GetColumnType());

        var persisted = status.ToString();
        Assert.True(Enum.TryParse<PropertyStatus>(persisted, false, out var parsed));
        Assert.Equal(status, parsed);
    }
}
