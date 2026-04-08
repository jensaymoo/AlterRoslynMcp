using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class NamespaceTraversalExtensions
{
    public static IEnumerable<INamedTypeSymbol> EnumerateTypes(
        this INamespaceSymbol root, 
        bool includeGenerated = false)
    {
        return EnumerateTypesRecursive(root, includeGenerated);
    }
    
    private static IEnumerable<INamedTypeSymbol> EnumerateTypesRecursive(
        ISymbol symbol, 
        bool includeGenerated)
    {
        switch (symbol)
        {
            case INamespaceSymbol ns:
                foreach (var member in ns.GetMembers())
                {
                    switch (member)
                    {
                        case INamedTypeSymbol type when !includeGenerated && type.IsImplicitlyDeclared:
                            continue;
                            
                        case INamedTypeSymbol type:
                            yield return type;
                            
                            foreach (var nested in EnumerateTypesRecursive(type, includeGenerated))
                            {
                                yield return nested;
                            }
                            break;
                            
                        case INamespaceSymbol nestedNs:
                            foreach (var nested in EnumerateTypesRecursive(nestedNs, includeGenerated))
                            {
                                yield return nested;
                            }
                            break;
                    }
                }
                break;
                
            case INamedTypeSymbol type when !includeGenerated && type.IsImplicitlyDeclared:
                break;
                
            case INamedTypeSymbol type:
                yield return type;
                
                foreach (var nested in type.GetTypeMembers())
                {
                    foreach (var nestedType in EnumerateTypesRecursive(nested, includeGenerated))
                    {
                        yield return nestedType;
                    }
                }
                break;
        }
    }
}
