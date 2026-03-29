namespace RoslynMcp.Core.Models;

public sealed record FindCodeSmellsResult(
    CodeSmellsSummary Summary,
    IReadOnlyList<CodeSmellFindingEntry> Findings,
    IReadOnlyList<string>? Warnings,
    ResultContextMetadata Context,
    ErrorInfo? Error = null);