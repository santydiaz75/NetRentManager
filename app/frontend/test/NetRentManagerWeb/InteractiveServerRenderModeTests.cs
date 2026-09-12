namespace NetRentManagerWebTests;

public class InteractiveServerRenderModeTests
{
    [Fact]
    public void InteractivePages_DeclareInteractiveServerExactlyOnce()
    {
        var home = ReadSource("src", "NetRentManagerWeb", "Components", "Pages", "Home.razor");
        var detail = ReadSource("src", "NetRentManagerWeb", "Features", "Properties", "Detail", "PropertyDetailPage.razor");

        Assert.Equal(1, CountOccurrences(home, "@rendermode InteractiveServer"));
        Assert.Equal(1, CountOccurrences(detail, "@rendermode InteractiveServer"));
    }

    [Fact]
    public void StaticHosts_DoNotDeclareInteractiveServer()
    {
        var app = ReadSource("src", "NetRentManagerWeb", "Components", "App.razor");
        var routes = ReadSource("src", "NetRentManagerWeb", "Components", "Routes.razor");
        var error = ReadSource("src", "NetRentManagerWeb", "Components", "Pages", "Error.razor");
        var notFound = ReadSource("src", "NetRentManagerWeb", "Components", "Pages", "NotFound.razor");

        Assert.DoesNotContain("InteractiveServer", app);
        Assert.DoesNotContain("InteractiveServer", routes);
        Assert.DoesNotContain("InteractiveServer", error);
        Assert.DoesNotContain("InteractiveServer", notFound);
    }

    private static string ReadSource(params string[] segments)
    {
        var path = Path.GetFullPath(Path.Combine(
            new[] { AppContext.BaseDirectory, "..", "..", "..", "..", ".." }
                .Concat(segments)
                .ToArray()));

        return File.ReadAllText(path);
    }

    private static int CountOccurrences(string value, string search)
        => value.Split(new[] { search }, StringSplitOptions.None).Length - 1;
}
