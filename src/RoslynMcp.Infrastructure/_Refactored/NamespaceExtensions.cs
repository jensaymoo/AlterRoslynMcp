using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class NamespaceExtensions
{
    public static IEnumerable<INamedTypeSymbol> EnumerateTypes(
        this INamespaceSymbol root,
        bool includeGenerated = false)
    {
        foreach (var member in root.GetMembers())
        {
            switch (member)
            {
                case INamespaceSymbol ns:
                    foreach (var type in ns.EnumerateTypes(includeGenerated))
                        yield return type;
                    break;

                case INamedTypeSymbol type:
                    foreach (var t in EnumerateType(type, includeGenerated))
                        yield return t;
                    break;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateType(INamedTypeSymbol type, bool includeGenerated)
    {
        if (!includeGenerated && type.IsImplicitlyDeclared)
            yield break;

        yield return type;

        foreach (var nested in type.GetTypeMembers())
        foreach (var t in EnumerateType(nested, includeGenerated))
            yield return t;
    }
}
