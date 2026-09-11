namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class MigrationShapeTests
{
    [Fact]
    public void Property_Migrations_Contain_Only_The_Approved_Functional_Migrations()
    {
        var migrationDirectory = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Infrastructure/Persistence/Migrations"));

        Assert.True(Directory.Exists(migrationDirectory), "No existe el directorio de migraciones.");

        var migrationFiles = Directory
            .GetFiles(migrationDirectory, "*.cs", SearchOption.TopDirectoryOnly)
            .Where(file =>
                !file.EndsWith("ModelSnapshot.cs", StringComparison.OrdinalIgnoreCase)
                && !file.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Equal(2, migrationFiles.Count);
        Assert.Contains(migrationFiles, file => file.EndsWith("AddPropertyManagementEntities.cs", StringComparison.Ordinal));
        Assert.Contains(migrationFiles, file => file.EndsWith("AllowNullPropertyImageUrl.cs", StringComparison.Ordinal));
    }

    [Fact]
    public void AddPropertyManagementEntities_Contains_Expected_Up_And_Down_Operations()
    {
        var migrationDirectory = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Infrastructure/Persistence/Migrations"));

        var migrationFile = Directory
            .GetFiles(migrationDirectory, "*AddPropertyManagementEntities.cs", SearchOption.TopDirectoryOnly)
            .Single();

        var migrationSource = File.ReadAllText(migrationFile);

        Assert.Contains("CreateTable(", migrationSource, StringComparison.Ordinal);
        Assert.Contains("name: \"property_statuses\"", migrationSource, StringComparison.Ordinal);
        Assert.Contains("name: \"properties\"", migrationSource, StringComparison.Ordinal);
        Assert.Contains("ck_properties_price_non_negative", migrationSource, StringComparison.Ordinal);
        Assert.Contains("ck_properties_bedroom_count_non_negative", migrationSource, StringComparison.Ordinal);
        Assert.Contains("ck_properties_bathroom_count_non_negative", migrationSource, StringComparison.Ordinal);
        Assert.Contains("ck_properties_area_square_meters_positive", migrationSource, StringComparison.Ordinal);
        Assert.Contains("protected override void Down", migrationSource, StringComparison.Ordinal);
        Assert.Contains("DropTable(", migrationSource, StringComparison.Ordinal);
    }

    [Fact]
    public void ModelSnapshot_Contains_Properties_And_Statuses_Tables()
    {
        var migrationDirectory = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Infrastructure/Persistence/Migrations"));

        var snapshotFile = Directory
            .GetFiles(migrationDirectory, "*ModelSnapshot.cs", SearchOption.TopDirectoryOnly)
            .Single();

        var snapshotSource = File.ReadAllText(snapshotFile);

        Assert.Contains("property_statuses", snapshotSource, StringComparison.Ordinal);
        Assert.Contains("properties", snapshotSource, StringComparison.Ordinal);
        Assert.Contains("Property<string>(\"Status\")", snapshotSource, StringComparison.Ordinal);
    }
}
