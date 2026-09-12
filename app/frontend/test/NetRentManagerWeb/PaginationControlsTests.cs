using System.IO;

namespace NetRentManagerWeb;

public class PaginationControlsTests
{
    private static string ComponentPath()
        => Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../../src/NetRentManagerWeb/Features/Properties/List/PaginationControls.razor"));

    [Fact]
    public void PaginationControls_BuildsHrefWithPageAndPageSize_ForEnabledLinks()
    {
        var content = File.ReadAllText(ComponentPath());

        Assert.Contains("href=\"@BuildHref(Page - 1)\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"@BuildHref(Page + 1)\"", content, StringComparison.Ordinal);
        Assert.Contains("$\"/?page={targetPage}&pageSize={PageSize}\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public void PaginationControls_RendersDisabledSpan_WhenHasPreviousOrHasNextIsFalse()
    {
        var content = File.ReadAllText(ComponentPath());

        Assert.Contains("@if (HasPrevious)", content, StringComparison.Ordinal);
        Assert.Contains("@if (HasNext)", content, StringComparison.Ordinal);
        Assert.Contains("aria-disabled=\"true\"", content, StringComparison.Ordinal);
    }
}
