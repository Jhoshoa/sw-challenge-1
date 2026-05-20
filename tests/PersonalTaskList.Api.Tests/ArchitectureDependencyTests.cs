namespace PersonalTaskList.Api.Tests;

public class ArchitectureDependencyTests
{
    [Fact]
    public void DomainLayerDoesNotDependOnOuterLayersOrFrameworks()
    {
        var repositoryRoot = FindRepositoryRoot();
        var domainFiles = Directory.EnumerateFiles(
            Path.Combine(repositoryRoot, "src", "PersonalTaskList.Api", "Domain"),
            "*.cs",
            SearchOption.AllDirectories);

        AssertDoesNotContainAny(domainFiles,
        [
            "Microsoft.",
            "System.Net",
            "PersonalTaskList.Api.Application",
            "PersonalTaskList.Api.Infrastructure",
            "PersonalTaskList.Api.Presentation"
        ]);
    }

    [Fact]
    public void ApplicationLayerDoesNotDependOnInfrastructureOrPresentation()
    {
        var repositoryRoot = FindRepositoryRoot();
        var applicationFiles = Directory.EnumerateFiles(
            Path.Combine(repositoryRoot, "src", "PersonalTaskList.Api", "Application"),
            "*.cs",
            SearchOption.AllDirectories);

        AssertDoesNotContainAny(applicationFiles,
        [
            "PersonalTaskList.Api.Infrastructure",
            "PersonalTaskList.Api.Presentation",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore"
        ]);
    }

    [Fact]
    public void HttpControllersLiveInPresentationLayer()
    {
        var repositoryRoot = FindRepositoryRoot();
        var sourceFiles = Directory.EnumerateFiles(
            Path.Combine(repositoryRoot, "src", "PersonalTaskList.Api"),
            "*.cs",
            SearchOption.AllDirectories);

        var filesWithControllerActions = sourceFiles
            .Where(file => File.ReadAllText(file).Contains("[ApiController]", StringComparison.Ordinal)
                || File.ReadAllText(file).Contains("[HttpGet", StringComparison.Ordinal)
                || File.ReadAllText(file).Contains("[HttpPost", StringComparison.Ordinal)
                || File.ReadAllText(file).Contains("[HttpPut", StringComparison.Ordinal)
                || File.ReadAllText(file).Contains("[HttpPatch", StringComparison.Ordinal)
                || File.ReadAllText(file).Contains("[HttpDelete", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(filesWithControllerActions);
        Assert.All(filesWithControllerActions, file => Assert.Contains(
            Path.Combine("Presentation", "Controllers"),
            file,
            StringComparison.Ordinal));
    }

    private static void AssertDoesNotContainAny(IEnumerable<string> files, IReadOnlyList<string> forbiddenText)
    {
        foreach (var file in files)
        {
            var source = File.ReadAllText(file);

            foreach (var text in forbiddenText)
            {
                Assert.DoesNotContain(text, source, StringComparison.Ordinal);
            }
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "PersonalTaskList.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not find repository root.");
    }
}
