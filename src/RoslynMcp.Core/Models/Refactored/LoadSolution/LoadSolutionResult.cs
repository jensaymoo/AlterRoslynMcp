namespace RoslynMcp.Core.Models;

public sealed record WorkspaceReadiness(
    string State,
    IReadOnlyList<string> DegradedReasons,
    string? RecommendedNextStep = null);

public sealed record LoadSolutionResult(
    string? SelectedSolutionPath,
    string WorkspaceId,
    string WorkspaceSnapshotId,
    IReadOnlyList<ProjectSummary> Projects,
    DiagnosticsSummary BaselineDiagnostics,
    WorkspaceReadiness Readiness,
    ErrorInfo? Error = null);