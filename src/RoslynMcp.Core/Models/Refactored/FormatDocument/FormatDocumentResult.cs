namespace RoslynMcp.Core.Models;

public sealed record FormatDocumentResult(
    string Path,
    bool WasFormatted,
    ErrorInfo? Error = null);