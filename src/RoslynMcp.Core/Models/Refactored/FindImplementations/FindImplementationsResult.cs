namespace RoslynMcp.Core.Models;

public sealed record FindImplementationsResult(CompactSymbolSummary? Symbol, IReadOnlyList<CompactSymbolSummary> Implementations, ErrorInfo? Error = null);