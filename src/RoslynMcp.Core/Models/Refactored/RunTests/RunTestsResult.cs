namespace RoslynMcp.Core.Models;

public sealed record RunTestsResult(
    string Outcome,
    int? ExitCode,
    IReadOnlyList<TestFailureGroup> FailureGroups,
    IReadOnlyList<BuildDiagnostic>? BuildDiagnostics = null,
    string? Summary = null,
    ErrorInfo? Error = null,
    TestRunCounts? Counts = null);