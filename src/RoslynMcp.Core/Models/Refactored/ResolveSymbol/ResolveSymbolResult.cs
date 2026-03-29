namespace RoslynMcp.Core.Models;

public sealed record ResolveSymbolResult(
    ResolvedSymbolSummary? Symbol,
    bool IsAmbiguous,
    IReadOnlyList<ResolveSymbolCandidate> Candidates,
    ErrorInfo? Error = null);