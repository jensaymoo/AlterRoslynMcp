using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ITypeEnumerationService : IScopedService
{
    Task<IEnumerable<TypeEntry>> EnumerateTypesAsync(CancellationToken ct  = default);
    Task<IEnumerable<TypeEntry>> EnumerateTypesAsync(string projectName, CancellationToken ct = default);
}