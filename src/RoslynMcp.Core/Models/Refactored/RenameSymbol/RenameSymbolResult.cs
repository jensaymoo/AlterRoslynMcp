namespace RoslynMcp.Core.Models;

public sealed record RenameSymbolResult(
    string? RenamedSymbolId,
    int ChangedDocumentCount,
    IReadOnlyList<AffectedFileLocations> AffectedLocationFiles,
    IReadOnlyList<string> ChangedFiles,
    ErrorInfo? Error = null);