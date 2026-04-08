using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface ITypeEnumerationService : IScopedService
{
    Task<IEnumerable<TypeEntry>> EnumerateTypesAsync(Project project, bool includeSummary, CancellationToken ct);
}