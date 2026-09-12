using System.IO;

namespace NetRentManagerWeb;

public class ScreenStateStylesTests
{
    [Theory]
    [InlineData(".state-loading")]
    [InlineData(".state-empty")]
    [InlineData(".state-error")]
    [InlineData(".state-success")]
    public void AppCss_Defines_ScreenState_Class(string expectedClass)
    {
        var path = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerWeb/wwwroot/app.css"));
        var content = File.ReadAllText(path);

        Assert.Contains(expectedClass, content, StringComparison.Ordinal);
    }
}
