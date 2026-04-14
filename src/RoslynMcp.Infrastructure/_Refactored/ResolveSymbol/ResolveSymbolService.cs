using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public class ResolveSymbolService(
    ISolutionWorkspaceService workspaceService,
    ISymbolResolverService symbolResolver) : IResolveSymbolService
{
    public async Task<IEnumerable<ResolvedSymbolEntry>> ResolveAsync(string symbolId, CancellationToken ct)
    {
        var solution = workspaceService.GetCurrentSolution();

        var results = (await Task.WhenAll(
            solution.Projects.Select(async project =>
            {
                try { return (ResolvedSymbolEntry?)new ResolvedSymbolEntry(await FindSymbolAsync(symbolId, project, ct), project); }
                catch (SymbolEntryNotFoundException) { return null; }
            })
        )).OfType<ResolvedSymbolEntry>().ToList();

        return results.Count > 0 ? results
            : throw new SymbolEntryNotFoundException($"Symbol '{symbolId}' not found in solution");
    }

    private async Task<ISymbol> FindSymbolAsync(string symbolId, Project project, CancellationToken ct)
    {
        try { return await symbolResolver.GetNamedTypeAsync(symbolId, project, ct); }
        catch (TypeEntryNotFoundException) { }

        try { return await symbolResolver.GetMemberAsync(symbolId, project, ct); }
        catch (MemberEntryNotFoundException) { }

        throw new SymbolEntryNotFoundException($"Symbol '{symbolId}' not found in project '{project.Name}'");
    }
}
