namespace RoslynMcp.Core.Models;

public sealed record ResolveSymbolBatchEntry(
    string? SymbolId = null,
    string? Path = null,
    int? Line = null,
    int? Column = null,
    string? QualifiedName = null,
    string? ProjectPath = null,
    string? ProjectName = null,
    string? ProjectId = null,
    string? Label = null);

public sealed record ResolveSymbolsBatchRequest(IReadOnlyList<ResolveSymbolBatchEntry> Entries);