using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class PropertyImageNullabilityTests
{
    [Fact]
    public void ImageUrl_IsNullable_WhileCoreFieldsRemainRequired()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);

        var entity = context.Model.FindEntityType(typeof(Property));
        Assert.NotNull(entity);
        Assert.True(entity.FindProperty(nameof(Property.ImageUrl))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(Property.Title))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(Property.Description))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(Property.Address))!.IsNullable);
    }
}
