using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.SymbolDisplay;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class MembersEnumerationService(
    ILogger<MembersEnumerationService> logger,
    ITypeResolverService typeResolverService,
    ISolutionWorkspaceService solutionWorkspaceService) : IMembersEnumerationService
{
    public Task<IEnumerable<MemberEntry>> EnumerateMembersAsync(
        string fullTypeName,
        MemberEntryKind? kind,
        SymbolAccessibility? accessibility,
        string? binding,
        bool includeInherited,
        bool includeSummary,
        CancellationToken ct)
    {
        try
        {
            var solution = solutionWorkspaceService.GetCurrentSolution();
            return EnumerateMembersInternalAsync(solution, fullTypeName, kind, accessibility, binding, includeInherited, includeSummary, ct);
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
        MemberEntryKind? kindFilter,
        SymbolAccessibility? accessibilityFilter,
        string? bindingFilter,
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
            ? CollectMembersWithInheritance(typeSymbol)
            : typeSymbol.GetMembers();

        var entries = members
            .Select(m => ToMemberEntry(m, typeSymbol, includeSummary))
            .Where(e => FilterByKind(e, kindFilter))
            .Where(e => FilterByAccessibility(e, accessibilityFilter))
            .Where(e => FilterByBinding(e, bindingFilter))
            .OrderBy(e => e.Kind)
            .ThenBy(e => e.DisplayName)
            .ThenBy(e => e.Signature);

        return entries;
    }

    private static ImmutableArray<ISymbol> CollectMembersWithInheritance(INamedTypeSymbol type)
    {
        var builder = ImmutableArray.CreateBuilder<ISymbol>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        static IEnumerable<INamedTypeSymbol> Traverse(INamedTypeSymbol current)
        {
            yield return current;
            var baseType = current.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
            foreach (var iface in current.AllInterfaces.OrderBy(static i => i.ToDisplayString(), StringComparer.Ordinal))
                yield return iface;
        }

        foreach (var declaringType in Traverse(type))
        {
            foreach (var member in declaringType.GetMembers())
            {
                if (!IsValidMemberKind(member))
                    continue;
                var key = GetKey(member);
                if (seen.Add(key))
                    builder.Add(member);
            }
        }

        return builder.ToImmutable();
    }

    private static bool IsValidMemberKind(ISymbol member)
    {
        return member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } => true,
            IMethodSymbol method when method.MethodKind == MethodKind.Ordinary 
                || method.MethodKind == MethodKind.UserDefinedOperator
                || method.MethodKind == MethodKind.Conversion 
                || method.MethodKind == MethodKind.ReducedExtension
                || method.MethodKind == MethodKind.DelegateInvoke => true,
            IPropertySymbol => true,
            IFieldSymbol field when !field.IsImplicitlyDeclared => true,
            IEventSymbol => true,
            _ => false
        };
    }

    private static MemberEntry ToMemberEntry(ISymbol member, INamedTypeSymbol sourceType, bool includeSummary)
    {
        var location = GetDeclarationLocation(member);
        var isInherited = member.ContainingType != null && !SymbolEqualityComparer.Default.Equals(member.ContainingType, sourceType);

        return new MemberEntry
        {
            DisplayName = GetDisplayName(member),
            Signature = GetSignature(member),
            Kind = GetMemberKind(member),
            Accessibility = GetAccessibility(member),
            IsStatic = member.IsStatic,
            IsInherited = isInherited,
            Location = location,
            Summary = includeSummary ? GetSummary(member) : null
        };
    }

    private static string GetDisplayName(ISymbol member)
    {
        if (member.Kind == SymbolKind.Method && member is IMethodSymbol { MethodKind: MethodKind.Constructor } constructor)
            return constructor.ContainingType.Name;
        return member.Name;
    }

    private static string GetSignature(ISymbol member)
    {
        return member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } constructor
                => $"{constructor.ContainingType.Name}({FormatParameters(constructor.Parameters)})",
            IMethodSymbol method => FormatMethodSignature(method),
            IPropertySymbol property => $"{property.Type.ToDisplayString()} {property.Name}",
            IFieldSymbol field => $"{field.Type.ToDisplayString()} {field.Name}",
            IEventSymbol @event => $"{@event.Type.ToDisplayString()} {@event.Name}",
            _ => member.Name
        };
    }

    private static string FormatMethodSignature(IMethodSymbol method)
    {
        var returnType = method.ReturnType.ToDisplayString();
        var parameters = FormatParameters(method.Parameters);
        return $"{returnType} {method.Name}({parameters})";
    }

    private static string FormatParameters(ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(", ", parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
    }

    private static MemberEntryKind GetMemberKind(ISymbol member)
    {
        return member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } => MemberEntryKind.Constructor,
            IMethodSymbol => MemberEntryKind.Method,
            IPropertySymbol => MemberEntryKind.Property,
            IFieldSymbol => MemberEntryKind.Field,
            IEventSymbol => MemberEntryKind.Event,
            _ => MemberEntryKind.Method
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

    private static SourceLocation? GetDeclarationLocation(ISymbol member)
    {
        var location = member.Locations.FirstOrDefault(l => l.IsInSource);
        if (location == null)
            return null;

        return new SourceLocation
        {
            FilePath = location.SourceTree?.FilePath ?? "",
            Line = location.GetLineSpan().StartLinePosition.Line + 1,
            Column = location.GetLineSpan().StartLinePosition.Character + 1
        };
    }

    private static string? GetSummary(ISymbol member)
    {
        return null;
    }

    private static bool FilterByKind(MemberEntry entry, MemberEntryKind? kind)
    {
        return kind == null || entry.Kind == kind;
    }

    private static bool FilterByAccessibility(MemberEntry entry, SymbolAccessibility? accessibility)
    {
        return accessibility == null || entry.Accessibility == accessibility;
    }

    private static bool FilterByBinding(MemberEntry entry, string? binding)
    {
        if (string.IsNullOrEmpty(binding))
            return true;

        return binding.ToLowerInvariant() switch
        {
            "static" => entry.IsStatic,
            "instance" => !entry.IsStatic,
            _ => true
        };
    }

    private static string GetKey(ISymbol member)
    {
        return member.Kind switch
        {
            SymbolKind.Method => $"M:{member.Name}",
            SymbolKind.Property => $"P:{member.Name}",
            SymbolKind.Field => $"F:{member.Name}",
            SymbolKind.Event => $"E:{member.Name}",
            _ => member.Name
        };
    }
}
