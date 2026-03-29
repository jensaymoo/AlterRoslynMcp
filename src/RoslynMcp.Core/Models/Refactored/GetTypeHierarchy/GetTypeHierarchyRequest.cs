namespace RoslynMcp.Core.Models;

public sealed record GetTypeHierarchyRequest(string SymbolId, bool? IncludeTransitive = null, int? MaxDerived = null);