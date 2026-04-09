using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ITypeEnumerationService : IScopedService
{
    Task<IEnumerable<TypeEntry>> EnumerateTypesBySolutionAsync(bool includeSummary, CancellationToken ct);
}