namespace RoslynMcp.Core.Models;

public sealed record ReplaceMethodBodyResult(
    string Status,
    IReadOnlyList<string> ChangedFiles,
    string TargetMethodSymbolId,
    ReplacedMethodBodyInfo? ReplacedMethodBody,
    DiagnosticsDeltaInfo DiagnosticsDelta,
    ErrorInfo? Error = null);