namespace RoslynMcp.Core.Models;

public sealed record ResolveSymbolRequest(
    string? SymbolId = null,
    string? Path = null,
    int? Line = null,
    int? Column = null,
    string? QualifiedName = null,
    string? ProjectPath = null,
    string? ProjectName = null,
    string? ProjectId = null);