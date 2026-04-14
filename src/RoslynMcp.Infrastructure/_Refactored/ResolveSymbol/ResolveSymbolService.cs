using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public class ResolveSymbolService(ISolutionWorkspaceService workspaceService) : IResolveSymbolService
{
    public async Task<IEnumerable<ResolvedSymbolEntry>> ResolveAsync(string symbolId, CancellationToken ct)
    {
        var solution = workspaceService.GetCurrentSolution();
        var results = new List<ResolvedSymbolEntry>();

        foreach (var project in solution.Projects)
        {
            if (await project.GetCompilationAsync(ct) is not { } compilation)
                continue;

            var symbol = FindSymbol(compilation, symbolId);
            if (symbol == null)
                continue;

            results.Add(new ResolvedSymbolEntry(symbol, project));
        }

        return results;
    }

    private static ISymbol? FindSymbol(Compilation compilation, string symbolId)
    {
        var types = compilation.GlobalNamespace
            .EnumerateTypes(includeGenerated: false)
            .Where(t => t.DeclaringSyntaxReferences.Any());

        // Поиск как тип
        var type = types.FirstOrDefault(t => t.GetDocumentationCommentId() == symbolId);
        if (type != null)
            return type;

        // Поиск как член типа
        return types
            .SelectMany(t => t.GetMembers())
            .FirstOrDefault(m => m.GetDocumentationCommentId() == symbolId);
    }
}
