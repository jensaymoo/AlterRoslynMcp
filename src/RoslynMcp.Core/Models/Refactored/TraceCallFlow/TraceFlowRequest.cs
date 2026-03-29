namespace RoslynMcp.Core.Models;

public sealed record TraceFlowRequest(
    string? SymbolId = null,
    string? Path = null,
    int? Line = null,
    int? Column = null,
    string? Direction = null,
    int? Depth = null,
    bool IncludePossibleTargets = false);