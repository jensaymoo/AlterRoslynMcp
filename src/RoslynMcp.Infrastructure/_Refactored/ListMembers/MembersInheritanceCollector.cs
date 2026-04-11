using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public class MembersInheritanceCollector(INamedTypeSymbol type)
{
    public ImmutableArray<ISymbol> CollectWithInheritance()
    {
        var seen = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        return [
            ..Traverse(type)
                .SelectMany(t => t.GetMembers())
                .Where(IsValidMemberKind)
                .Where(seen.Add)
        ];
    }

    private static IEnumerable<INamedTypeSymbol> Traverse(INamedTypeSymbol currentType)
    {
        yield return currentType;

        var baseType = currentType.BaseType;
        while (baseType != null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }

        foreach (var iface in currentType.AllInterfaces.OrderBy(i => i.ToDisplayString(), StringComparer.Ordinal))
            yield return iface;
    }

    private static bool IsValidMemberKind(ISymbol member) => member switch
    {
        IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } => true,
        IMethodSymbol { MethodKind: MethodKind.Ordinary
            or MethodKind.UserDefinedOperator
            or MethodKind.Conversion
            or MethodKind.ReducedExtension
            or MethodKind.DelegateInvoke } => true,
        IPropertySymbol => true,
        IFieldSymbol { IsImplicitlyDeclared: false } => true,
        IEventSymbol => true,
        _ => false
    };
}