namespace RoslynMcp.Core.Models;

public sealed record FindCodeSmellsRequest(
    string Path,
    int? MaxFindings = null,
    IReadOnlyList<string>? RiskLevels = null,
    IReadOnlyList<string>? Categories = null,
    string? ReviewMode = null);