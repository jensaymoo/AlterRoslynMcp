using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ISymbolResolverService : IScopedService
{
    Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolId, Solution solution, CancellationToken ct = default);
    Task<INamedTypeSymbol?> GetNamedTypeAsync(string symbolId, Project project, CancellationToken ct = default);

    Task<ISymbol?> GetMemberAsync(string symbolId, Solution solution, CancellationToken ct = default);
    Task<ISymbol?> GetMemberAsync(string symbolId, Project project, CancellationToken ct = default);
}
