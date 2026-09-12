using System.IO;

namespace NetRentManagerWeb;

public class MainLayoutCompositionTests
{
    private static string ComponentsPath()
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerWeb/Components"));

    [Fact]
    public void MainLayout_Renders_AppShell_Sidebar_And_Main()
    {
        var path = Path.Combine(ComponentsPath(), "Layout", "MainLayout.razor");
        var content = File.ReadAllText(path);

        Assert.Contains("app-shell", content, StringComparison.Ordinal);
        Assert.Contains("app-sidebar", content, StringComparison.Ordinal);
        Assert.Contains("app-main", content, StringComparison.Ordinal);
        Assert.Contains("app-content", content, StringComparison.Ordinal);
        Assert.Contains("NavMenu", content, StringComparison.Ordinal);
    }

    [Fact]
    public void NavMenu_Defines_Inicio_And_Propiedades_Links()
    {
        var path = Path.Combine(ComponentsPath(), "Layout", "NavMenu.razor");
        var content = File.ReadAllText(path);

        Assert.Contains("Inicio", content, StringComparison.Ordinal);
        Assert.Contains("Propiedades", content, StringComparison.Ordinal);
        Assert.Contains("sidebar-link", content, StringComparison.Ordinal);
    }
}
