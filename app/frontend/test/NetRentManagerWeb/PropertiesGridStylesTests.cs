using System.IO;

namespace NetRentManagerWeb;

public class PropertiesGridStylesTests
{
    [Theory]
    [InlineData(".properties-grid")]
    [InlineData("grid-template-columns: repeat(3, 1fr)")]
    [InlineData("grid-template-columns: repeat(2, 1fr)")]
    public void AppCss_Defines_PropertiesGrid_ResponsiveColumns(string expected)
    {
        var path = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerWeb/wwwroot/app.css"));
        var content = File.ReadAllText(path);

        Assert.Contains(expected, content, StringComparison.Ordinal);
    }
}
