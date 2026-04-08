using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RoslynMcp.Infrastructure._Refactored;

public enum Severity
{
    Error,
    Warning,
    Info
}

public class Diagnostic
{
    public required string Code { get; init; }
    public required Severity Severity { get; init; }
    public required string Message { get; init; }
    public required SourceLocation? Location { get; init; }
}


public class AnalyzeSolutionService(ILogger<AnalyzeSolutionService> logger) : IAnalyzeSolutionService
{
    public async Task<IEnumerable<Diagnostic>> AnalyzeSolutionAsync(Solution solution, CancellationToken ct)
    {
        try
        {
            var diagnostics = new List<Diagnostic>();

            foreach (var project in solution.Projects)
            {
                if (await project.GetCompilationAsync(ct) is { } compilation)
                {
                    var projectDiagn = compilation.GetDiagnostics();
                    var dignItems = projectDiagn
                        .Where(d => d.Severity != DiagnosticSeverity.Hidden)
                        .Select(d => new Diagnostic() {
                            Code = d.Id,
                            Severity = d.Severity switch
                            {
                                DiagnosticSeverity.Error => Severity.Error,
                                DiagnosticSeverity.Warning => Severity.Warning,
                                _ => Severity.Info
                            },
                            Message = d.GetMessage(),
                            Location= d.Location.AsSourceLocation()}
                        );

                    diagnostics.AddRange(dignItems);
                }
            }

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