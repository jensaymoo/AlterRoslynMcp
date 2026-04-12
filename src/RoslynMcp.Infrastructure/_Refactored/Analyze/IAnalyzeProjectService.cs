using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public interface IAnalyzeProjectService : IScopedService
{
    Task<IEnumerable<Diagnostic>> AnalyzeProjectAsync(Project project, CancellationToken ct);
}