using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public class SymbolResolverService : ISymbolResolverService
{
    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolId, Solution solution, CancellationToken ct = default)
    {
        var results = await Task.WhenAll(
            solution.Projects.Select(prj => GetNamedTypeAsync(symbolId, prj, ct))
        );

        return results.FirstOrDefault(r => r != null)
               ?? throw new TypeEntryNotFoundException($"Type '{symbolId}' not found in solution");
    }

    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolId, Project project, CancellationToken ct = default)
    {
        if (await project.GetCompilationAsync(ct) is not { } compilation)
            return null;

        return compilation.GlobalNamespace
                   .EnumerateTypes(includeGenerated: false)
                   .FirstOrDefault(type =>
                       type.DeclaringSyntaxReferences.Any() &&
                       type.GetDocumentationCommentId() == symbolId)
               ?? throw new TypeEntryNotFoundException($"Type '{symbolId}' not found in project '{project.Name}'");
    }

    public async Task<ISymbol?> GetMemberAsync(string symbolId, Solution solution, CancellationToken ct = default)
    {
        var results = await Task.WhenAll(
            solution.Projects.Select(prj => GetMemberAsync(symbolId, prj, ct))
        );

        return results.FirstOrDefault(r => r != null)
               ?? throw new MemberEntryNotFoundException($"Member '{symbolId}' not found in solution");
    }

    public async Task<ISymbol?> GetMemberAsync(string symbolId, Project project, CancellationToken ct = default)
    {
        if (await project.GetCompilationAsync(ct) is not { } compilation)
            return null;

        return compilation.GlobalNamespace
                   .EnumerateTypes(includeGenerated: false)
                   .Where(t => t.DeclaringSyntaxReferences.Any())
                   .SelectMany(t => t.GetMembers())
                   .FirstOrDefault(m => m.GetDocumentationCommentId() == symbolId)
               ?? throw new MemberEntryNotFoundException($"Member '{symbolId}' not found in project '{project.Name}'");
    }
}
