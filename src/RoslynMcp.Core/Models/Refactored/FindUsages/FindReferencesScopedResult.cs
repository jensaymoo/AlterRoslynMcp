namespace RoslynMcp.Core.Models;

public sealed record FindReferencesScopedResult(UsageSymbolSummary? Symbol,
    IReadOnlyList<ReferenceFileGroup> ReferenceFiles,
    int TotalCount,
    ErrorInfo? Error = null);