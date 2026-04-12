using Microsoft.CodeAnalysis;

namespace RoslynMcp.Infrastructure._Refactored;

public static class SeverityExtension
{
    internal static Severity GetSeverity(this Microsoft.CodeAnalysis.Diagnostic diagnostic)
    {
        return diagnostic.Severity switch
        {
            DiagnosticSeverity.Error => Severity.Error,
            DiagnosticSeverity.Warning => Severity.Warning,
            _ => Severity.Info
        };
    }
}