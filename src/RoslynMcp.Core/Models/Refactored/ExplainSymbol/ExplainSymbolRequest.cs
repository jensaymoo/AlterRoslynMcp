namespace RoslynMcp.Core.Models;

public sealed record ExplainSymbolRequest(string? SymbolId = null, string? Path = null, int? Line = null, int? Column = null);