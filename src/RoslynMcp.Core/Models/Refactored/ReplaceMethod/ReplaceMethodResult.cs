namespace RoslynMcp.Core.Models;

public sealed record ReplaceMethodResult(
    string Status,
    IReadOnlyList<string> ChangedFiles,
    string TargetMethodSymbolId,
    ReplacedMethodInfo? ReplacedMethod,
    DiagnosticsDeltaInfo DiagnosticsDelta,
    ErrorInfo? Error = null);