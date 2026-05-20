using System.Text.RegularExpressions;

namespace PersonalTaskList.Api.Tests;

public class OpenApiContractTests
{
    [Fact]
    public void ImplementedEndpointsMatchOpenApiContract()
    {
        var repositoryRoot = FindRepositoryRoot();
        var openApiEndpoints = ReadOpenApiEndpoints(Path.Combine(repositoryRoot, "docs", "openapi.yaml"));
        var implementedEndpoints = ReadImplementedEndpoints(Path.Combine(repositoryRoot, "src", "PersonalTaskList.Api"));

        Assert.Equal(openApiEndpoints.Order(), implementedEndpoints.Order());
    }

    [Fact]
    public async Task RootEndpointIsNotExposed()
    {
        await using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    private static HashSet<string> ReadOpenApiEndpoints(string path)
    {
        var endpoints = new HashSet<string>(StringComparer.Ordinal);
        string? currentPath = null;
        var insidePaths = false;

        foreach (var line in File.ReadLines(path))
        {
            if (line == "paths:")
            {
                insidePaths = true;
                continue;
            }

            if (insidePaths && line == "components:")
            {
                break;
            }

            if (!insidePaths)
            {
                continue;
            }

            var pathMatch = Regex.Match(line, @"^  (/[^:]+):$");
            if (pathMatch.Success)
            {
                currentPath = pathMatch.Groups[1].Value;
                continue;
            }

            var methodMatch = Regex.Match(line, @"^    (get|post|put|patch|delete):$");
            if (methodMatch.Success && currentPath is not null)
            {
                endpoints.Add($"{methodMatch.Groups[1].Value.ToUpperInvariant()} {currentPath}");
            }
        }

        return endpoints;
    }

    private static HashSet<string> ReadImplementedEndpoints(string sourceDirectory)
    {
        var endpoints = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories))
        {
            var source = File.ReadAllText(file);
            var controllerRoute = Regex.Match(source, @"\[Route\(""([^""]+)""\)\]");
            if (!controllerRoute.Success)
            {
                continue;
            }

            var baseRoute = "/" + controllerRoute.Groups[1].Value.Trim('/');
            var matches = Regex.Matches(
                source,
                @"\[Http(Get|Post|Put|Patch|Delete)(?:\(""([^""]*)""\))?\]");

            foreach (Match match in matches)
            {
                var method = match.Groups[1].Value.ToUpperInvariant();
                var routeTemplate = match.Groups[2].Success ? match.Groups[2].Value : string.Empty;
                var route = CombineRoutes(baseRoute, routeTemplate)
                    .Replace("{id:guid}", "{id}", StringComparison.Ordinal);

                endpoints.Add($"{method} {route}");
            }
        }

        return endpoints;
    }

    private static string CombineRoutes(string baseRoute, string routeTemplate)
    {
        if (string.IsNullOrWhiteSpace(routeTemplate))
        {
            return baseRoute;
        }

        return $"{baseRoute}/{routeTemplate.Trim('/')}";
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "docs", "openapi.yaml")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not find repository root.");
    }
}
