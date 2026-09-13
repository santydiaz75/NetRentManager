namespace NetRentManagerWebTests;

public class InteractiveServerBehaviorTests
{
    [Fact]
    public void Home_PreservesExistingPaginationComponentWithoutDirectHttpClient()
    {
        var home = ReadSource("src", "NetRentManagerWeb", "Components", "Pages", "Home.razor");

        Assert.Contains("<PaginationControls", home, StringComparison.Ordinal);
        Assert.DoesNotContain("new HttpClient", home, StringComparison.Ordinal);
        Assert.DoesNotContain("RestService.For", home, StringComparison.Ordinal);
    }

    [Fact]
    public void PropertyDetail_PreservesReturnLinkToRoot()
    {
        var detail = ReadSource("src", "NetRentManagerWeb", "Features", "Properties", "Detail", "PropertyDetailPage.razor");

        Assert.Contains("href=\"/\"", detail, StringComparison.Ordinal);
        Assert.Contains("Volver al listado", detail, StringComparison.Ordinal);
    }

    [Fact]
    public void PropertyDetail_DoesNotApplyDetailGridToPageContainer()
    {
        var detail = ReadSource("src", "NetRentManagerWeb", "Features", "Properties", "Detail", "PropertyDetailPage.razor");

        Assert.Contains("<div class=\"page-container\">", detail, StringComparison.Ordinal);
        Assert.DoesNotContain("<div class=\"page-container property-detail\">", detail, StringComparison.Ordinal);
        Assert.Contains("<article class=\"card property-detail\"", detail, StringComparison.Ordinal);
    }

    private static string ReadSource(params string[] segments)
    {
        var path = Path.GetFullPath(Path.Combine(
            new[] { AppContext.BaseDirectory, "..", "..", "..", "..", ".." }
                .Concat(segments)
                .ToArray()));

        return File.ReadAllText(path);
    }
}
