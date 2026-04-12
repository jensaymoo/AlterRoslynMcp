using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class MembersEnumerationService(ILogger<MembersEnumerationService> logger, ITypeResolverService typeResolverService,
    ISolutionWorkspaceService solutionWorkspaceService) : IMembersEnumerationService
{
    public async Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string symbolName, MemberEntryKind? kind, SymbolAccessibility? accessibility, bool includeInherited,
        CancellationToken ct = default)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();
            var typeSymbol = await typeResolverService.GetNamedTypeAsync(symbolName, solution, ct)
                ?? throw new TypeEntryNotFoundException($"Type '{symbolName}' not found in solution");

            return FilterAndSort(typeSymbol, kind, accessibility, includeInherited);
        }
        catch (Exception ex) when (ex is not TypeEntryNotFoundException)
        {
            logger.LogError(ex, "Unexpected error during member enumeration");
            throw;
        }
    }

    public async Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string symbolName, string projectName, MemberEntryKind? kind, SymbolAccessibility? accessibility,
        bool includeInherited, CancellationToken ct = default)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();

            var project = solution.Projects
                .FirstOrDefault(p => p.Name.Equals(projectName.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? throw new ProjectNotFoundException($"Project '{projectName}' not found");

            var typeSymbol = await typeResolverService.GetNamedTypeAsync(symbolName, project, ct)
                ?? throw new TypeEntryNotFoundException($"Type '{symbolName}' not found in project '{projectName}'");

            return FilterAndSort(typeSymbol, kind, accessibility, includeInherited);
        }
        catch (Exception ex) when (ex is not TypeEntryNotFoundException and not ProjectNotFoundException)
        {
            logger.LogError(ex, "Unexpected error during member enumeration");
            throw;
        }
    }

    private static IEnumerable<MemberEntry> FilterAndSort(
        INamedTypeSymbol typeSymbol, MemberEntryKind? kind, SymbolAccessibility? accessibility, bool includeInherited) =>
        GetMembers(typeSymbol, includeInherited)
            .Select(m => new MemberEntry(m, typeSymbol))
            .Where(e => (kind == null || e.Kind == kind) && (accessibility == null || e.Accessibility == accessibility))
            .OrderBy(e => e.Kind)
            .ThenBy(e => e.SymbolName)
            .ThenBy(e => e.Signature);

    private static IEnumerable<ISymbol> GetMembers(INamedTypeSymbol typeSymbol, bool includeInherited) =>
        includeInherited
            ? new MembersInheritanceCollector(typeSymbol).CollectWithInheritance()
            : typeSymbol.GetMembers();
}