namespace RoslynMcp.Core.Models;

public sealed record AddMethodResult(
    string Status,
    IReadOnlyList<string> ChangedFiles,
    string TargetTypeSymbolId,
    AddedMethodInfo? AddedMethod,
    DiagnosticsDeltaInfo DiagnosticsDelta,
    ErrorInfo? Error = null);