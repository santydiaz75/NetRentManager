namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class MigrationStartupTests
{
    [Fact]
    public void Program_Calls_MigrateAsync_Before_Run()
    {
        var programPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Program.cs"));

        var source = File.ReadAllText(programPath);
        var migrateIndex = source.IndexOf("await app.MigrateAsync(", StringComparison.Ordinal);
        var runIndex = source.IndexOf("app.Run();", StringComparison.Ordinal);

        Assert.True(migrateIndex >= 0, "Program.cs debe invocar app.MigrateAsync().");
        Assert.True(runIndex > migrateIndex, "app.MigrateAsync() debe ejecutarse antes de app.Run().");
    }

    [Fact]
    public void MigrationExtensions_Does_Not_Invoke_DatabaseSeeder_Manually()
    {
        var extensionPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Infrastructure/Persistence/MigrationExtensions.cs"));

        var source = File.ReadAllText(extensionPath);

        Assert.Contains("Database.MigrateAsync", source, StringComparison.Ordinal);
        Assert.DoesNotContain("DatabaseSeeder", source, StringComparison.Ordinal);
    }
}
