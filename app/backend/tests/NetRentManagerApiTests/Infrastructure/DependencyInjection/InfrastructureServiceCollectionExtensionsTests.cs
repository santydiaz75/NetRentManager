using Microsoft.Extensions.Configuration;
using NetRentManagerApi.Infrastructure.DependencyInjection;

namespace NetRentManagerApiTests.Infrastructure.DependencyInjection;

public sealed class InfrastructureServiceCollectionExtensionsTests
{
    private static IConfiguration BuildConfiguration(
        string? connectionString,
        string? username,
        string? password)
    {
        var values = new Dictionary<string, string?>();

        if (connectionString is not null)
        {
            values["ConnectionStrings:DefaultConnection"] = connectionString;
        }

        if (username is not null)
        {
            values["DatabaseCredentials:Username"] = username;
        }

        if (password is not null)
        {
            values["DatabaseCredentials:Password"] = password;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    [Fact]
    public void BuildConnectionString_CombinesBaseConnectionString_WithCredentials()
    {
        var configuration = BuildConfiguration(
            "Host=db.example.com; Database=neondb; SSL Mode=VerifyFull; Channel Binding=Require;",
            "neondb_owner",
            "s3cr3t");

        var result = InfrastructureServiceCollectionExtensions.BuildConnectionString(configuration);

        Assert.Contains("Username=neondb_owner", result, StringComparison.Ordinal);
        Assert.Contains("Password=s3cr3t", result, StringComparison.Ordinal);
        Assert.Contains("Host=db.example.com", result, StringComparison.Ordinal);
        Assert.Contains("Database=neondb", result, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildConnectionString_Throws_WhenBaseConnectionStringMissing()
    {
        var configuration = BuildConfiguration(null, "neondb_owner", "s3cr3t");

        var exception = Assert.Throws<InvalidOperationException>(
            () => InfrastructureServiceCollectionExtensions.BuildConnectionString(configuration));

        Assert.Contains("DefaultConnection", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildConnectionString_Throws_WhenUsernameMissing()
    {
        var configuration = BuildConfiguration(
            "Host=db.example.com; Database=neondb;",
            null,
            "s3cr3t");

        var exception = Assert.Throws<InvalidOperationException>(
            () => InfrastructureServiceCollectionExtensions.BuildConnectionString(configuration));

        Assert.Contains("DatabaseCredentials:Username", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildConnectionString_Throws_WhenPasswordMissing()
    {
        var configuration = BuildConfiguration(
            "Host=db.example.com; Database=neondb;",
            "neondb_owner",
            null);

        var exception = Assert.Throws<InvalidOperationException>(
            () => InfrastructureServiceCollectionExtensions.BuildConnectionString(configuration));

        Assert.Contains("DatabaseCredentials:Password", exception.Message, StringComparison.Ordinal);
    }
}
