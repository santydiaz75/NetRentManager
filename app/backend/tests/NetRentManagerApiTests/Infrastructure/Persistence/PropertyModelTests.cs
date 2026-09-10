using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class PropertyModelTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=netrentmanager_model_tests;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void Model_Defines_Property_And_PropertyStatusCatalog_With_Expected_Schema()
    {
        using var context = CreateContext();

        var propertyEntity = context.Model.FindEntityType(typeof(Property));
        Assert.NotNull(propertyEntity);
        Assert.Equal("properties", propertyEntity.GetTableName());

        var statusEntity = context.Model.FindEntityType(typeof(PropertyStatusCatalogEntry));
        Assert.NotNull(statusEntity);
        Assert.Equal("property_statuses", statusEntity.GetTableName());
    }

    [Fact]
    public void Property_Columns_Constraints_And_Indexes_Are_Configured()
    {
        using var context = CreateContext();

        var entity = context.Model.FindEntityType(typeof(Property));
        Assert.NotNull(entity);

        var title = entity.FindProperty(nameof(Property.Title));
        Assert.NotNull(title);
        Assert.False(title.IsNullable);
        Assert.Equal(200, title.GetMaxLength());

        var description = entity.FindProperty(nameof(Property.Description));
        Assert.NotNull(description);
        Assert.False(description.IsNullable);
        Assert.Equal(2000, description.GetMaxLength());

        var address = entity.FindProperty(nameof(Property.Address));
        Assert.NotNull(address);
        Assert.False(address.IsNullable);
        Assert.Equal(300, address.GetMaxLength());

        var price = entity.FindProperty(nameof(Property.Price));
        Assert.NotNull(price);
        Assert.Equal(18, price.GetPrecision());
        Assert.Equal(2, price.GetScale());

        var area = entity.FindProperty(nameof(Property.AreaSquareMeters));
        Assert.NotNull(area);
        Assert.Equal(10, area.GetPrecision());
        Assert.Equal(2, area.GetScale());

        var createdAt = entity.FindProperty(nameof(Property.CreatedAt));
        Assert.NotNull(createdAt);
        Assert.False(createdAt.IsNullable);
        Assert.Equal("CURRENT_TIMESTAMP", createdAt.GetDefaultValueSql());

        var updatedAt = entity.FindProperty(nameof(Property.UpdatedAt));
        Assert.NotNull(updatedAt);
        Assert.True(updatedAt.IsNullable);

        var indexes = entity.GetIndexes().ToList();
        Assert.Contains(indexes, index =>
            index.GetDatabaseName() == "ix_properties_status"
            && index.Properties.Any(property => property.Name == nameof(Property.Status)));

    }

    [Fact]
    public void PropertyStatusCatalog_Has_Text_Key_And_Unique_Index()
    {
        using var context = CreateContext();

        var entity = context.Model.FindEntityType(typeof(PropertyStatusCatalogEntry));
        Assert.NotNull(entity);

        var key = entity.FindPrimaryKey();
        Assert.NotNull(key);
        Assert.Single(key.Properties);
        Assert.Equal(nameof(PropertyStatusCatalogEntry.Value), key.Properties[0].Name);

        var valueProperty = entity.FindProperty(nameof(PropertyStatusCatalogEntry.Value));
        Assert.NotNull(valueProperty);
        Assert.False(valueProperty.IsNullable);
        Assert.Equal(32, valueProperty.GetMaxLength());

        var descriptionProperty = entity.FindProperty(nameof(PropertyStatusCatalogEntry.Description));
        Assert.NotNull(descriptionProperty);
        Assert.False(descriptionProperty.IsNullable);
        Assert.Equal(300, descriptionProperty.GetMaxLength());

        var uniqueIndex = entity.GetIndexes().Single(index => index.GetDatabaseName() == "ix_property_statuses_value");
        Assert.True(uniqueIndex.IsUnique);
    }
}
