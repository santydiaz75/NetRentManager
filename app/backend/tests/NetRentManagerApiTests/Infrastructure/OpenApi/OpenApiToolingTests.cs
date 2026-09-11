using System.Diagnostics;

namespace NetRentManagerApiTests.Infrastructure.OpenApi;

public sealed class OpenApiToolingTests
{
    [Fact]
    public void RedoclyLint_SucceedsForGeneratedDocument()
    {
        var result = Run("node.exe", "node_modules/@redocly/cli/bin/cli.js lint --config .redocly.yaml app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json");

        Assert.True(result.ExitCode == 0, result.Output);
    }

    [Fact]
    public void Nswag_GeneratesSmokeClient()
    {
        var output = Path.Combine(FindRepositoryRoot(), "artifacts", "openapi-client-smoke", "NetRentManagerApiClient.cs");
        if (File.Exists(output))
        {
            File.Delete(output);
        }

        var configPath = Path.Combine(FindRepositoryRoot(), "support", "scripts", "openapi-v1.nswag.json");
        var result = Run("dotnet", $"tool run nswag run \"{configPath}\"");

        Assert.True(result.ExitCode == 0, result.Output);
        Assert.True(File.Exists(output), result.Output);
    }

    private static (int ExitCode, string Output) Run(string fileName, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = FindRepositoryRoot(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        using (process)
        {
            process.Start();
            var output = process.StandardOutput.ReadToEnd() + Environment.NewLine + process.StandardError.ReadToEnd();
            process.WaitForExit();
            return (process.ExitCode, output);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "package.json")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return directory.FullName;
    }
}
