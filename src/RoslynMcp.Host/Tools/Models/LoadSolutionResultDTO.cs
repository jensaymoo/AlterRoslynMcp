namespace RoslynMcp.Host.Tools.Models;

public record LoadSolutionResultDTO(string SolutionPath,
    IEnumerable<ProjectSummaryDTO> Projects,
    IEnumerable<DiagnosticDTO> BaselineDiagnostics);