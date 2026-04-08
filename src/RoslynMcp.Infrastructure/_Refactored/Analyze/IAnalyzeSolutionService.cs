using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface IAnalyzeSolutionService: IScopedService
{
    Task<IEnumerable<Diagnostic>> AnalyzeSolutionAsync(Solution solution, CancellationToken ct);
}