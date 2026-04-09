using RoslynMcp.Infrastructure._Refactored;

namespace RoslynMcp.Host.Tools.Models;

public record DiagnosticDTO(
    string Code,
    Severity Severity,
    string Message,
    SourceLocationDTO? Location
);