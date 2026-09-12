using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetRentManagerWeb.Services.Api;

namespace NetRentManagerWeb;

public class ApiClientRegistrationTests
{
    private static string RepoRoot()
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerWeb"));

    [Fact]
    public void AddNetRentManagerApiClient_Registers_NamedHttpClient_WithBaseAddress()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiSettings:BaseUrl"] = "https://localhost:7065"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddNetRentManagerApiClient(configuration);

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient(NetRentManagerApiClientRegistration.HttpClientName);

        Assert.Equal(new Uri("https://localhost:7065"), client.BaseAddress);
    }

    [Fact]
    public void AddNetRentManagerApiClient_Throws_WhenBaseUrlMissing()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(
            () => services.AddNetRentManagerApiClient(configuration));
    }

    [Fact]
    public void RazorComponents_Do_Not_Reference_HttpClient_Or_RestServiceFor_Directly()
    {
        var componentsPath = Path.Combine(RepoRoot(), "Components");
        var razorFiles = Directory.GetFiles(componentsPath, "*.razor", SearchOption.AllDirectories);
        var codeBehindFiles = Directory.GetFiles(componentsPath, "*.razor.cs", SearchOption.AllDirectories);

        foreach (var file in razorFiles.Concat(codeBehindFiles))
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain("new HttpClient(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("RestService.For", content, StringComparison.Ordinal);
        }
    }
}
