namespace RoslynMcp.Core.Models;

public sealed record DeleteMethodResult(
    string Status,
    IReadOnlyList<string> ChangedFiles,
    string TargetMethodSymbolId,
    DeletedMethodInfo? DeletedMethod,
    DiagnosticsDeltaInfo DiagnosticsDelta,
    ErrorInfo? Error = null);