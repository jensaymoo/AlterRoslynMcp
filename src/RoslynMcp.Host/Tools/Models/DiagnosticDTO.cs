using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Models;

public class DiagnosticDTO(
    string Code,
    Severity Severity,
    string Message,
    SourceLocationDTO? Location
);