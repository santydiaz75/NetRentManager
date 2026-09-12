using System.IO;
using System.Linq;

namespace NetRentManagerWeb;

public class IconSystemTests
{
    private static string RepoRoot()
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerWeb"));

    [Fact]
    public void NavMenu_References_LucideIcon_Components()
    {
        var path = Path.Combine(RepoRoot(), "Components", "Layout", "NavMenu.razor");
        var content = File.ReadAllText(path);

        Assert.Contains("<LucideIcon", content, StringComparison.Ordinal);
        Assert.Contains("icon icon-sm", content, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("bootstrap-icons")]
    [InlineData("bi bi-")]
    [InlineData("font-awesome")]
    [InlineData("fa fa-")]
    [InlineData("fluent-icons")]
    public void Project_Does_Not_Reference_OtherIconLibraries(string forbiddenToken)
    {
        var root = RepoRoot();
        var razorFiles = Directory.GetFiles(root, "*.razor", SearchOption.AllDirectories);
        var cssFiles = Directory.GetFiles(Path.Combine(root, "wwwroot"), "*.css", SearchOption.AllDirectories);

        foreach (var file in razorFiles.Concat(cssFiles))
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain(forbiddenToken, content, StringComparison.OrdinalIgnoreCase);
        }
    }
}
