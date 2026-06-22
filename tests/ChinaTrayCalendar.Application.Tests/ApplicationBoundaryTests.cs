using System.Xml.Linq;

namespace ChinaTrayCalendar.Application.Tests;

public sealed class ApplicationBoundaryTests
{
    private static readonly string[] ForbiddenSourceTokens =
    [
        "System.Windows",
        "Windows.Forms",
        "Microsoft.Win32",
        "Registry",
        "System.IO",
        "File.",
        "Directory.",
        "Path.",
        "System.Text.Json",
        "JsonSerializer",
        "DateTime.Now",
        "DateTime.UtcNow",
        "TimeZoneInfo.Local",
    ];

    [Fact]
    public void ApplicationProjectReferencesOnlyDomain()
    {
        string projectPath = Path.Combine(
            GetRepositoryRoot(),
            "src",
            "ChinaTrayCalendar.Application",
            "ChinaTrayCalendar.Application.csproj");
        XDocument project = XDocument.Load(projectPath);

        string[] references = project
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(value => value is not null)
            .Cast<string>()
            .Order()
            .ToArray();

        string[] expected =
        [
            @"..\ChinaTrayCalendar.Domain\ChinaTrayCalendar.Domain.csproj",
        ];

        Assert.Equal(expected, references);
    }

    [Fact]
    public void ApplicationSourceDoesNotUseForbiddenDependencies()
    {
        string sourceRoot = Path.Combine(GetRepositoryRoot(), "src", "ChinaTrayCalendar.Application");
        string[] files = Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !IsBuildOutput(sourceRoot, file))
            .ToArray();
        List<string> violations = [];

        foreach (string file in files)
        {
            string content = File.ReadAllText(file);
            violations.AddRange(ForbiddenSourceTokens
                .Where(content.Contains)
                .Select(token => $"{Path.GetRelativePath(sourceRoot, file)} contains {token}"));
        }

        Assert.Empty(violations);
    }

    private static bool IsBuildOutput(string sourceRoot, string file)
    {
        string relativePath = Path.GetRelativePath(sourceRoot, file);

        return relativePath.StartsWith($"bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            || relativePath.StartsWith($"obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal);
    }

    private static string GetRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Dateview.slnx")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException("Repository root could not be found.");
        }

        return directory.FullName;
    }
}
