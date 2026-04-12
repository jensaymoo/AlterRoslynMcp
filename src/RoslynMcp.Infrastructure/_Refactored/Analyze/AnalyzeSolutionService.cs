using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public class AnalyzeSolutionService(ILogger<AnalyzeSolutionService> logger) : IAnalyzeSolutionService
{
    public async Task<IEnumerable<Diagnostic>> AnalyzeSolutionAsync(Solution solution, CancellationToken ct = default)
    {
        try
        {
            var compilations = await Task.WhenAll(
                    solution.Projects.Select(p => p.GetCompilationAsync(ct))
                );

            var diagnostics = compilations
                .OfType<Compilation>()
                .SelectMany(c => c.GetDiagnostics())
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Unknown analyzing error");
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