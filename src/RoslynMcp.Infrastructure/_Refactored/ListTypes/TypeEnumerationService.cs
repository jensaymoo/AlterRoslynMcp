using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class TypeEnumerationService(
    ILogger<TypeEnumerationService> logger,
    ISolutionWorkspaceService solutionWorkspaceService) : ITypeEnumerationService
{
    public async Task<IEnumerable<TypeEntry>> EnumerateTypesAsync(CancellationToken ct = default)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();
            
            var types = await Task.WhenAll(
                    solution.Projects.Select(p => EnumerateProjectTypesAsync(p, ct))
                );
            
            return types.SelectMany(x => x)
                .OrderBy(x => x.SymbolName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during type enumeration");
            throw;
        }
    }

    public async Task<IEnumerable<TypeEntry>> EnumerateTypesAsync(string projectName, CancellationToken ct = default)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();

            var project = solution.Projects
                .FirstOrDefault(p => p.Name.Equals(projectName)) 
                          ?? throw new ProjectNotFoundException($"Project '{projectName}' not found");

            var types = await EnumerateProjectTypesAsync(project, ct);
            
            return types.OrderBy(x => x.SymbolName);
        }
        catch (Exception ex) when (ex is not ProjectNotFoundException)
        {
            logger.LogError(ex, "Unexpected error during type enumeration");
            throw;
        }
    }

    private async Task<IEnumerable<TypeEntry>> EnumerateProjectTypesAsync(Project project, CancellationToken ct = default)
    {
        if (await project.GetCompilationAsync(ct) is not { } compilation)
            return [];

        var projectTreeSet = (await GetProjectSyntaxTreesAsync(project, ct))
            .OfType<SyntaxTree>()
            .ToHashSet();

        return compilation.GlobalNamespace
            .EnumerateTypes(includeGenerated: false)
            .Where(t => t.DeclaringSyntaxReferences
                .Select(r => r.SyntaxTree)
                .Any(projectTreeSet.Contains))
            .Select(t => new TypeEntry(t, project));
    }

    private static async Task<SyntaxTree?[]> GetProjectSyntaxTreesAsync(Project project, CancellationToken ct = default)
        => await Task.WhenAll(
            project.Documents
                .Where(d => d.SupportsSyntaxTree)
                .Select(d => d.GetSyntaxTreeAsync(ct))
        );
}