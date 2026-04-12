using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class AnalyzeProjectService(ILogger<AnalyzeProjectService> logger) : IAnalyzeProjectService
{
    public async Task<IEnumerable<Diagnostic>> AnalyzeProjectAsync(Project project, CancellationToken ct = default)
    {
        try
        {
            if (await project.GetCompilationAsync(ct) is { } compilation)
            {
                var diagnostics = compilation.GetDiagnostics()
                    .Where(d => d.Severity != DiagnosticSeverity.Hidden)
                    .Select(d => new Diagnostic
                    {
                        Code = d.Id,
                        Severity = d.GetSeverity(),
                        Message = d.GetMessage(),
                        Location = d.Location.AsSourceLocation()
                    });

                return OrderDiagnostics(diagnostics);
            }

            return Enumerable.Empty<Diagnostic>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing project {ProjectName}", project.Name);
            throw;
        }
    }

    private IEnumerable<Diagnostic> OrderDiagnostics(IEnumerable<Diagnostic> diagnostics)
        => diagnostics
            .OrderBy(static item => item.Location?.FilePath, StringComparer.Ordinal)
            .ThenBy(static item => item.Severity)
            .ThenBy(static item => item.Code, StringComparer.Ordinal)
            .ToList();
}