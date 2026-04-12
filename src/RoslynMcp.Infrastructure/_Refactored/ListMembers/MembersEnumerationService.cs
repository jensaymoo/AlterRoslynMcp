using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class MembersEnumerationService(ILogger<MembersEnumerationService> logger, ITypeResolverService typeResolverService,
    ISolutionWorkspaceService solutionWorkspaceService) : IMembersEnumerationService
{
    public async Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string symbolName, MemberEntryKind? kind, SymbolAccessibility? accessibility, bool includeInherited, CancellationToken ct = default)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();
            
            var typeSymbol = await typeResolverService.GetNamedTypeAsync(symbolName, solution, ct);
            if (typeSymbol == null)
                return [];

            return GetMembers(typeSymbol, includeInherited)
                .Select(m => new MemberEntry(m, typeSymbol))
                .Where(e => kind == null || e.Kind == kind)
                .Where(e => accessibility == null || e.Accessibility == accessibility)
                .OrderBy(e => e.Kind)
                .ThenBy(e => e.SymbolName)
                .ThenBy(e => e.Signature);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during member enumeration");
            throw;
        }
    }

    private static IEnumerable<ISymbol> GetMembers(INamedTypeSymbol typeSymbol, bool includeInherited) =>
        includeInherited
            ? new MembersInheritanceCollector(typeSymbol).CollectWithInheritance()
            : typeSymbol.GetMembers();
}