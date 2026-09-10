namespace NetRentManagerApiTests.Infrastructure;

public class ProgramCompositionTests
{
    [Fact]
    public void Program_ComposesInfrastructure_WithoutManualEndpointRegistration()
    {
        var programPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Program.cs"));

        var source = File.ReadAllText(programPath);

        Assert.Contains("AddInfrastructure", source, StringComparison.Ordinal);
        Assert.Contains("MapSliceEndpoints", source, StringComparison.Ordinal);
        Assert.DoesNotContain("MapGet(\"/health\"", source, StringComparison.Ordinal);
        Assert.DoesNotContain("MapControllers", source, StringComparison.Ordinal);
        Assert.DoesNotContain("DbContext", source, StringComparison.Ordinal);
    }
}