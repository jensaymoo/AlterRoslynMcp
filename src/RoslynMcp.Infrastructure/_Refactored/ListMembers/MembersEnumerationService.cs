using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class MembersEnumerationService(
    ILogger<MembersEnumerationService> logger,
    ITypeResolverService typeResolverService,
    ISolutionWorkspaceService solutionWorkspaceService,
    IMemberExtractor memberExtractor,
    IMembersInheritanceCollector inheritanceCollector) : IMembersEnumerationService
{
    public Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string fullTypeName,
        MemberEntryKind? kind,
        SymbolAccessibility? accessibility,
        bool includeInherited,
        bool includeSummary,
        CancellationToken ct)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();
            return EnumerateMembersInternalAsync(solution, fullTypeName, kind, accessibility, includeInherited, includeSummary, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during member enumeration");
            throw;
        }
    }

    private async Task<IEnumerable<MemberEntry>> EnumerateMembersInternalAsync(
        Solution solution,
        string fullTypeName,
        MemberEntryKind? kind,
        SymbolAccessibility? accessibility,
        bool includeInherited,
        bool includeSummary,
        CancellationToken ct)
    {
        var typeSymbol = await typeResolverService.GetNamedTypeAsync(fullTypeName, solution);
        if (typeSymbol == null)
        {
            return [];
        }

        var members = includeInherited
            ? inheritanceCollector.CollectWithInheritance(typeSymbol)
            : typeSymbol.GetMembers();

        var entries = members
            .Select(m => ToMemberEntry(m, typeSymbol, includeSummary))
            .Where(e => FilterByKind(e, kind))
            .Where(e => FilterByAccessibility(e, accessibility))
            .OrderBy(e => e.Kind)
            .ThenBy(e => e.DisplayName)
            .ThenBy(e => e.Signature);

        return entries;
    }

    private MemberEntry ToMemberEntry(ISymbol member, INamedTypeSymbol sourceType, bool includeSummary)
    {
        var isInherited = member.ContainingType != null && !SymbolEqualityComparer.Default.Equals(member.ContainingType, sourceType);

        return new MemberEntry
        {
            DisplayName = memberExtractor.GetDisplayName(member),
            Signature = memberExtractor.GetSignature(member),
            Kind = memberExtractor.GetKind(member),
            Accessibility = GetAccessibility(member),
            IsStatic = memberExtractor.GetIsStatic(member),
            IsInherited = isInherited,
            Location = member.Locations.FirstOrDefault(l => l.IsInSource)?.AsSourceLocation(),
            Summary = includeSummary ? GetSummary(member) : null
        };
    }

    private static SymbolAccessibility GetAccessibility(ISymbol member)
    {
        return member.DeclaredAccessibility switch
        {
            Microsoft.CodeAnalysis.Accessibility.Internal => SymbolAccessibility.Internal,
            Microsoft.CodeAnalysis.Accessibility.Private => SymbolAccessibility.Private,
            Microsoft.CodeAnalysis.Accessibility.Protected => SymbolAccessibility.Protected,
            Microsoft.CodeAnalysis.Accessibility.Public => SymbolAccessibility.Public,
            Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal => SymbolAccessibility.PrivateProtected,
            Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal => SymbolAccessibility.ProtectedInternal,
            _ => SymbolAccessibility.NotApplicable
        };
    }

    private static bool FilterByKind(MemberEntry entry, MemberEntryKind? kind)
    {
        return kind == null || entry.Kind == kind;
    }

    private static bool FilterByAccessibility(MemberEntry entry, SymbolAccessibility? accessibility)
    {
        return accessibility == null || entry.Accessibility == accessibility;
    }

    private static string? GetSummary(ISymbol member)
    {
        return null;
    }
}
