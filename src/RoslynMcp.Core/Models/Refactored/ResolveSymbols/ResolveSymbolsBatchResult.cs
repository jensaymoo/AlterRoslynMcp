namespace RoslynMcp.Core.Models;

public sealed record ResolveSymbolsBatchResult(
    IReadOnlyList<ResolveSymbolsBatchItemResult> Results,
    int TotalCount,
    int ResolvedCount,
    int AmbiguousCount,
    int ErrorCount,
    ErrorInfo? Error = null);