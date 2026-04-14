using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public class SymbolResolverService : ISymbolResolverService
{
    public async Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolId, Solution solution, CancellationToken ct = default)
    {
        var results = await Task.WhenAll(
            solution.Projects.Select(async prj =>
            {
                try { return await GetNamedTypeAsync(symbolId, prj, ct); }
                catch (TypeEntryNotFoundException) { return null; }
            })
        );
        return results.OfType<INamedTypeSymbol>().FirstOrDefault()
               ?? throw new TypeEntryNotFoundException(symbolId);
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
               ?? throw new TypeEntryNotFoundException(symbolId, project.Name);
    }

    public async Task<ISymbol?> GetMemberAsync(string symbolId, Solution solution, CancellationToken ct = default)
    {
        var results = await Task.WhenAll(
            solution.Projects.Select(async prj =>
            {
                try { return await GetMemberAsync(symbolId, prj, ct); }
                catch (MemberEntryNotFoundException) { return null; }
            })
        );
        return results.OfType<ISymbol>().FirstOrDefault()
               ?? throw new MemberEntryNotFoundException(symbolId);
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
               ?? throw new MemberEntryNotFoundException(symbolId, project.Name);
    }
}
