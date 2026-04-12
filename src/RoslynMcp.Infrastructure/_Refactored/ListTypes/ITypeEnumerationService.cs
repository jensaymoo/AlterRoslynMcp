using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ITypeEnumerationService : IScopedService
{
    Task<IEnumerable<TypeEntry>> EnumerateTypesBySolutionAsync(CancellationToken ct  = default);
    Task<IEnumerable<TypeEntry>> EnumerateTypesByProjectAsync(string projectName, CancellationToken ct = default);
}