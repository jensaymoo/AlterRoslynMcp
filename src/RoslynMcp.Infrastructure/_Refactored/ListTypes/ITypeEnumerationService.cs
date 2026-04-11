using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ITypeEnumerationService : IScopedService
{
    Task<IEnumerable<TypeEntry>> EnumerateTypesBySolutionAsync(CancellationToken ct);
}