using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ITypeResolverService : IScopedService
{
    string GetDisplayName(INamedTypeSymbol namedType);
    string GetDisplayNamespace(INamedTypeSymbol namedType);
    Task<INamedTypeSymbol?> GetNamedTypeAsync(string displayName, Solution solution);
    Task<INamedTypeSymbol?> GetNamedTypeAsync(string displayName, Project project);
}
