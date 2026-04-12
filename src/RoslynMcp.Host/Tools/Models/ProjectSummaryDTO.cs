namespace RoslynMcp.Host.Tools.Models;

public record ProjectSummaryDTO(
    string Name, 
    string? ProjectPath,
    IEnumerable<DiagnosticDTO>? Diagnostics);