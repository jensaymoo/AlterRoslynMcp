namespace RoslynMcp.Core.Models;

public sealed record FindReferencesScopedRequest(string SymbolId, string Scope, string? Path = null);