namespace RoslynMcp.Core.Models;

public sealed record GetTypeHierarchyResult(CompactSymbolSummary? Symbol,
    IReadOnlyList<CompactSymbolSummary> BaseTypes,
    IReadOnlyList<CompactSymbolSummary> ImplementedInterfaces,
    IReadOnlyList<CompactSymbolSummary> DerivedTypes,
    ErrorInfo? Error = null);