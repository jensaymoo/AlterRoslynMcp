using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ITypeResolverService : IScopedService
{
    Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolName, Solution solution, CancellationToken ct = default);
    Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolName, Project project, CancellationToken ct = default);
}
