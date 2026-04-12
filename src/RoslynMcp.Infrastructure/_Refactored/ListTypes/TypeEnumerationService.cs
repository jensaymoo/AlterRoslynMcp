using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class TypeEnumerationService(
    ILogger<TypeEnumerationService> logger,
    ISolutionWorkspaceService solutionWorkspaceService) : ITypeEnumerationService
{
    public async Task<IEnumerable<TypeEntry>> EnumerateTypesBySolutionAsync(CancellationToken ct = default)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();

            var allTypes = await Task.WhenAll(
                solution.Projects
                    .Select(p => EnumerateProjectTypesAsync(p, ct))
                );

            return allTypes
                .SelectMany(x => x)
                .OrderBy(x => x.SymbolName, StringComparer.Ordinal)
                .ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during type enumeration");
            throw;
        }
    }

    private async Task<IEnumerable<TypeEntry>> EnumerateProjectTypesAsync(Project project, CancellationToken ct  = default)
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