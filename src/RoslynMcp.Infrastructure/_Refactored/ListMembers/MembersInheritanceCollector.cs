using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public class MembersInheritanceCollector : IMembersInheritanceCollector
{
    public ImmutableArray<ISymbol> CollectWithInheritance(INamedTypeSymbol type)
    {
        var seen = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
        var builder = ImmutableArray.CreateBuilder<ISymbol>();

        var allTypes = Traverse(type).ToArray();

        foreach (var declaringType in allTypes)
        {
            foreach (var member in declaringType.GetMembers())
            {
                if (!IsValidMemberKind(member))
                    continue;

                if (seen.Add(member))
                    builder.Add(member);
            }
        }

        return builder.ToImmutable();
    }

    private static IEnumerable<INamedTypeSymbol> Traverse(INamedTypeSymbol type)
    {
        yield return type;

        var baseType = type.BaseType;
        while (baseType != null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }

        foreach (var iface in type.AllInterfaces.OrderBy(i => i.ToDisplayString(), StringComparer.Ordinal))
            yield return iface;
    }

    private static bool IsValidMemberKind(ISymbol member)
    {
        return member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } => true,
            IMethodSymbol method when method.MethodKind is MethodKind.Ordinary
                or MethodKind.UserDefinedOperator
                or MethodKind.Conversion
                or MethodKind.ReducedExtension
                or MethodKind.DelegateInvoke => true,
            IPropertySymbol => true,
            IFieldSymbol field when !field.IsImplicitlyDeclared => true,
            IEventSymbol => true,
            _ => false
        };
    }
}
