namespace RoslynMcp.Core.Models;

public sealed record TraceFlowResult(
    string? RootSymbolId,
    TraceRootSummary? Root,
    string Direction,
    int Depth,
    IReadOnlyDictionary<string, TraceSymbolEntry>? Symbols,
    IReadOnlyList<TraceFlowEdge> Edges,
    IReadOnlyList<TraceFlowEdge>? PossibleTargetEdges = null,
    IReadOnlyList<FlowTransition>? Transitions = null,
    IReadOnlyList<string>? RootUncertaintyCategories = null,
    ErrorInfo? Error = null);