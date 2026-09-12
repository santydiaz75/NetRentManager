using System.IO;

namespace NetRentManagerWeb;

public class ThirdPartyDependencyTests
{
    private static string RepoRoot()
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerWeb"));

    [Theory]
    [InlineData("bootstrap")]
    [InlineData("font-awesome")]
    [InlineData("fontawesome")]
    [InlineData("fluent-icons")]
    [InlineData("bulma")]
    [InlineData("tailwind")]
    public void AppRazor_Does_Not_Reference_ThirdParty_Frameworks(string forbiddenToken)
    {
        var appRazorPath = Path.Combine(RepoRoot(), "Components", "App.razor");
        var content = File.ReadAllText(appRazorPath);

        Assert.DoesNotContain(forbiddenToken, content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WwwwrootLib_Does_Not_Contain_Bootstrap_Or_ThirdParty_Icon_Libraries()
    {
        var libPath = Path.Combine(RepoRoot(), "wwwroot", "lib");

        if (!Directory.Exists(libPath))
        {
            return;
        }

        var entries = Directory.GetFileSystemEntries(libPath, "*", SearchOption.AllDirectories);

        Assert.DoesNotContain(entries, entry =>
            entry.Contains("bootstrap", StringComparison.OrdinalIgnoreCase)
            || entry.Contains("font-awesome", StringComparison.OrdinalIgnoreCase)
            || entry.Contains("fontawesome", StringComparison.OrdinalIgnoreCase)
            || entry.Contains("fluent-icons", StringComparison.OrdinalIgnoreCase));
    }
}
